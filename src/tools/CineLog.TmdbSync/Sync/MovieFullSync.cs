using CineLog.Domain.Enums;
using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Infrastructure;
using DM.MovieApi.MovieDb.Discover;
using DM.MovieApi.MovieDb.Movies;
using Microsoft.EntityFrameworkCore;
using Movie = CineLog.Domain.Entities.Movie;
using MovieCast = CineLog.Domain.Entities.MovieCast;
using MovieCrew = CineLog.Domain.Entities.MovieCrew;
using Person = CineLog.Domain.Entities.Person;
using ProductionCompany = CineLog.Domain.Entities.ProductionCompany;
using MovieProductionCompany = CineLog.Domain.Entities.MovieProductionCompany;

namespace CineLog.TmdbSync.Sync;

public class MovieFullSync(
    TmdbSyncDbContext db,
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
            await rateLimiter.ThrottleAsync(ct);
            var detail = await RetryHelper.ExecuteAsync(() => movieApi.FindByIdAsync(info.Id), ct: ct);
            if (detail.Item is null) return;

            var d = detail.Item;
            var existing = await db.Movies.FirstOrDefaultAsync(m => m.TmdbId == info.Id, ct);

            if (existing is null)
            {
                var movie = Movie.Create(info.Id, info.Title, MovieType.Movie);
                movie.UpdateDetails(
                    info.Overview,
                    info.PosterPath,
                    info.BackdropPath,
                    info.ReleaseDate == default ? null : DateOnly.FromDateTime(info.ReleaseDate),
                    d.Runtime,
                    d.Genres.Select(g => g.Name),
                    d.ImdbId,
                    d.OriginalTitle,
                    d.OriginalLanguage,
                    d.Tagline,
                    d.Status,
                    (long?)d.Budget,
                    (decimal?)d.Revenue);
                movie.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
                movie.UpdatePopularity(info.Popularity);
                db.Movies.Add(movie);
                await db.SaveChangesAsync(ct);

                await SyncCreditsAsync(movie.Id, info.Id, ct);
                await SyncProductionCompaniesAsync(movie.Id, d.ProductionCompanies, ct);
            }
            else
            {
                existing.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
                existing.UpdatePopularity(info.Popularity);

                if (!existing.IsManuallyEdited)
                {
                    existing.UpdateDetails(
                        info.Overview,
                        info.PosterPath,
                        info.BackdropPath,
                        info.ReleaseDate == default ? null : DateOnly.FromDateTime(info.ReleaseDate),
                        d.Runtime,
                        d.Genres.Select(g => g.Name),
                        d.ImdbId,
                        d.OriginalTitle,
                        d.OriginalLanguage,
                        d.Tagline,
                        d.Status,
                        (long?)d.Budget,
                        (decimal?)d.Revenue);
                }

                await db.SaveChangesAsync(ct);
                await SyncCreditsAsync(existing.Id, info.Id, ct);
                await SyncProductionCompaniesAsync(existing.Id, d.ProductionCompanies, ct);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to sync movie TmdbId={TmdbId}", info.Id);
            await failures.RecordAsync(SyncType, info.Id, ex, ct);
        }
    }

    private async Task SyncCreditsAsync(Guid movieId, int tmdbId, CancellationToken ct)
    {
        await rateLimiter.ThrottleAsync(ct);
        var credits = await RetryHelper.ExecuteAsync(() => movieApi.GetCreditsAsync(tmdbId), ct: ct);
        if (credits.Item is null) return;

        var oldCast = await db.MovieCast.Where(c => c.MovieId == movieId).ToListAsync(ct);
        var oldCrew = await db.MovieCrew.Where(c => c.MovieId == movieId).ToListAsync(ct);
        db.MovieCast.RemoveRange(oldCast);
        db.MovieCrew.RemoveRange(oldCrew);

        foreach (var member in credits.Item.CastMembers)
        {
            await EnsurePersonAsync(member.PersonId, member.Name, ct);
            db.MovieCast.Add(new MovieCast
            {
                MovieId = movieId,
                PersonId = member.PersonId,
                Character = member.Character,
                Order = member.Order,
                CreditId = member.CreditId
            });
        }

        foreach (var member in credits.Item.CrewMembers)
        {
            await EnsurePersonAsync(member.PersonId, member.Name, ct);
            db.MovieCrew.Add(new MovieCrew
            {
                MovieId = movieId,
                PersonId = member.PersonId,
                Department = member.Department,
                Job = member.Job,
                CreditId = member.CreditId
            });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task SyncProductionCompaniesAsync(
        Guid movieId,
        IEnumerable<DM.MovieApi.MovieDb.Companies.ProductionCompanyInfo> companies,
        CancellationToken ct)
    {
        var oldLinks = await db.MovieProductionCompanies.Where(mp => mp.MovieId == movieId).ToListAsync(ct);
        db.MovieProductionCompanies.RemoveRange(oldLinks);

        foreach (var c in companies)
        {
            if (!await db.ProductionCompanies.AnyAsync(pc => pc.Id == c.Id, ct))
                db.ProductionCompanies.Add(new ProductionCompany { Id = c.Id, Name = c.Name });
            db.MovieProductionCompanies.Add(new MovieProductionCompany { MovieId = movieId, CompanyId = c.Id });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task EnsurePersonAsync(int personId, string name, CancellationToken ct)
    {
        if (!await db.Persons.AnyAsync(p => p.Id == personId, ct))
            db.Persons.Add(new Person { Id = personId, Name = name, SyncedAt = DateTimeOffset.UtcNow });
    }
}
