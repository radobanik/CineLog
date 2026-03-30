using CineLog.Domain.Enums;
using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Entities;
using CineLog.TmdbSync.Infrastructure;
using DM.MovieApi.MovieDb.Discover;
using DM.MovieApi.MovieDb.Movies;
using Microsoft.EntityFrameworkCore;
using DomainMovie = CineLog.Domain.Entities.Movie;

namespace CineLog.TmdbSync.Sync;

public class MovieFullSync(
    TmdbSyncDbContext appDb,
    TmdbSchemaDbContext tmdbDb,
    IApiMovieRequest movieApi,
    IApiDiscoverRequest discoverApi,
    TmdbRateLimiter rateLimiter,
    CheckpointService checkpoints,
    FailureTracker failures,
    ILogger<MovieFullSync> logger,
    IConfiguration configuration)
{
    private const string SyncType = "movies";
    private readonly int _maxPages = configuration.GetValue("Sync:MaxDiscoverPages", 500);
    private readonly int _batchSize = configuration.GetValue("Sync:BatchSize", 5);

    public async Task SyncAsync(CancellationToken ct)
    {
        logger.LogInformation("Starting full movie sync");
        var startPage = await checkpoints.GetLastPageAsync(SyncType, ct) + 1;
        var builder = new DiscoverMovieParameterBuilder().SortBy(DiscoverSortBy.Popularity, SortDirection.Desc);

        for (var page = startPage; page <= _maxPages && !ct.IsCancellationRequested; page++)
        {
            await rateLimiter.ThrottleAsync(ct);
            var response = await RetryHelper.ExecuteAsync(
                () => discoverApi.DiscoverMoviesAsync(builder, page),
                ct: ct);

            if (!response.Results.Any()) break;

            var tasks = response.Results
                .Select(info => SyncMovieAsync(info, ct))
                .Chunk(_batchSize)
                .Select(batch => Task.WhenAll(batch));

            foreach (var batch in tasks)
            {
                if (ct.IsCancellationRequested) break;
                await batch;
            }

            await checkpoints.SaveAsync(SyncType, page, response.TotalPages, ct);
            logger.LogInformation("Movies: page {Page}/{Total}", page, response.TotalPages);

            if (page >= response.TotalPages) break;
        }

        await checkpoints.ResetAsync(SyncType, ct);
        logger.LogInformation("Movie sync complete");
    }

    private async Task SyncMovieAsync(MovieInfo info, CancellationToken ct)
    {
        try
        {
            await SyncPublicMovieAsync(info, ct);
            await SyncMovieDetailsAsync(info.Id, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to sync movie TmdbId={TmdbId}", info.Id);
            await failures.RecordAsync(SyncType, info.Id, ex, ct);
        }
    }

    private async Task SyncPublicMovieAsync(MovieInfo info, CancellationToken ct)
    {
        var existing = await appDb.Movies.FirstOrDefaultAsync(m => m.TmdbId == info.Id, ct);

        if (existing is null)
        {
            await rateLimiter.ThrottleAsync(ct);
            var detail = await RetryHelper.ExecuteAsync(() => movieApi.FindByIdAsync(info.Id), ct: ct);

            var movie = DomainMovie.Create(info.Id, info.Title, MovieType.Movie);
            movie.UpdateDetails(
                info.Overview,
                info.PosterPath,
                info.BackdropPath,
                info.ReleaseDate == default ? null : DateOnly.FromDateTime(info.ReleaseDate),
                detail.Item?.Runtime,
                detail.Item?.Genres.Select(g => g.Name).ToList() ?? []);
            movie.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
            appDb.Movies.Add(movie);
        }
        else
        {
            existing.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
        }

        await appDb.SaveChangesAsync(ct);
    }

    private async Task SyncMovieDetailsAsync(int tmdbId, CancellationToken ct)
    {
        await rateLimiter.ThrottleAsync(ct);
        var detail = await RetryHelper.ExecuteAsync(() => movieApi.FindByIdAsync(tmdbId), ct: ct);
        if (detail.Item is null) return;

        var d = detail.Item;

        // Upsert movie_details
        var existing = await tmdbDb.MovieDetails.FindAsync([tmdbId], ct);
        if (existing is null)
        {
            tmdbDb.MovieDetails.Add(new TmdbMovieDetail
            {
                TmdbId = tmdbId,
                ImdbId = d.ImdbId,
                OriginalTitle = d.OriginalTitle,
                OriginalLanguage = d.OriginalLanguage,
                Budget = (int)d.Budget,
                Revenue = d.Revenue,
                Tagline = d.Tagline,
                Status = d.Status,
                Popularity = d.Popularity,
                SyncedAt = DateTimeOffset.UtcNow
            });
        }
        else
        {
            existing.ImdbId = d.ImdbId;
            existing.OriginalTitle = d.OriginalTitle;
            existing.Budget = (int)d.Budget;
            existing.Revenue = d.Revenue;
            existing.Tagline = d.Tagline;
            existing.Status = d.Status;
            existing.Popularity = d.Popularity;
            existing.SyncedAt = DateTimeOffset.UtcNow;
        }

        // Sync genres
        await SyncMovieGenresAsync(tmdbId, d.Genres.Select(g => g.Id).ToList(), ct);

        // Sync production companies
        await SyncMovieProductionCompaniesAsync(tmdbId, d.ProductionCompanies, ct);

        // Sync cast/crew
        await SyncMovieCreditsAsync(tmdbId, ct);

        await tmdbDb.SaveChangesAsync(ct);
    }

    private async Task SyncMovieGenresAsync(int tmdbId, List<int> genreIds, CancellationToken ct)
    {
        var existingLinks = await tmdbDb.MovieGenres
            .Where(mg => mg.TmdbMovieId == tmdbId)
            .ToListAsync(ct);
        tmdbDb.MovieGenres.RemoveRange(existingLinks);

        foreach (var gid in genreIds)
        {
            if (!await tmdbDb.Genres.AnyAsync(g => g.Id == gid, ct))
                tmdbDb.Genres.Add(new TmdbGenre { Id = gid, Name = "Unknown", IsMovieGenre = true });
            tmdbDb.MovieGenres.Add(new TmdbMovieGenre { TmdbMovieId = tmdbId, GenreId = gid });
        }
    }

    private async Task SyncMovieProductionCompaniesAsync(
        int tmdbId,
        IEnumerable<DM.MovieApi.MovieDb.Companies.ProductionCompanyInfo> companies,
        CancellationToken ct)
    {
        var existingLinks = await tmdbDb.MovieProductionCompanies
            .Where(mp => mp.TmdbMovieId == tmdbId)
            .ToListAsync(ct);
        tmdbDb.MovieProductionCompanies.RemoveRange(existingLinks);

        foreach (var c in companies)
        {
            if (!await tmdbDb.ProductionCompanies.AnyAsync(pc => pc.Id == c.Id, ct))
                tmdbDb.ProductionCompanies.Add(new TmdbProductionCompany { Id = c.Id, Name = c.Name });
            tmdbDb.MovieProductionCompanies.Add(new TmdbMovieProductionCompany { TmdbMovieId = tmdbId, CompanyId = c.Id });
        }
    }

    private async Task SyncMovieCreditsAsync(int tmdbId, CancellationToken ct)
    {
        await rateLimiter.ThrottleAsync(ct);
        var credits = await RetryHelper.ExecuteAsync(() => movieApi.GetCreditsAsync(tmdbId), ct: ct);
        if (credits.Item is null) return;

        // Remove old credits
        var oldCast = await tmdbDb.MovieCast.Where(c => c.TmdbMovieId == tmdbId).ToListAsync(ct);
        var oldCrew = await tmdbDb.MovieCrew.Where(c => c.TmdbMovieId == tmdbId).ToListAsync(ct);
        tmdbDb.MovieCast.RemoveRange(oldCast);
        tmdbDb.MovieCrew.RemoveRange(oldCrew);

        foreach (var member in credits.Item.CastMembers)
        {
            await EnsurePersonAsync(member.PersonId, member.Name, ct);
            tmdbDb.MovieCast.Add(new TmdbMovieCast
            {
                TmdbMovieId = tmdbId,
                PersonId = member.PersonId,
                Character = member.Character,
                Order = member.Order,
                CreditId = member.CreditId
            });
        }

        foreach (var member in credits.Item.CrewMembers)
        {
            await EnsurePersonAsync(member.PersonId, member.Name, ct);
            tmdbDb.MovieCrew.Add(new TmdbMovieCrew
            {
                TmdbMovieId = tmdbId,
                PersonId = member.PersonId,
                Department = member.Department,
                Job = member.Job,
                CreditId = member.CreditId
            });
        }
    }

    private async Task EnsurePersonAsync(int personId, string name, CancellationToken ct)
    {
        if (!await tmdbDb.Persons.AnyAsync(p => p.Id == personId, ct))
        {
            tmdbDb.Persons.Add(new TmdbPerson
            {
                Id = personId,
                Name = name,
                SyncedAt = DateTimeOffset.UtcNow
            });
        }
    }
}
