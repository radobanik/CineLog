using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Infrastructure;
using DM.MovieApi.MovieDb.TV;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Sync;

public class TvSeriesFullSync(
    TmdbSyncDbContext db,
    IApiTVShowRequest tvApi,
    TmdbDirectClient directClient,
    TmdbRateLimiter rateLimiter,
    CheckpointService checkpoints,
    FailureTracker failures,
    ILogger<TvSeriesFullSync> logger,
    IConfiguration configuration)
{
    private const string SyncType = "tv";
    private readonly int _maxPages = configuration.GetValue("Sync:MoviesMaxDiscoverPages", 500);

    public async Task SyncAsync(CancellationToken ct)
    {
        logger.LogInformation("Starting full TV series sync");
        var startPage = await checkpoints.GetLastPageAsync(SyncType, ct) + 1;

        for (var page = startPage; (_maxPages == -1 || page <= _maxPages) && !ct.IsCancellationRequested; page++)
        {
            await rateLimiter.ThrottleAsync(ct);
            var response = await RetryHelper.ExecuteAsync(
                () => tvApi.GetPopularAsync(page),
                ct: ct);

            if (!response.Results.Any()) break;

            foreach (var info in response.Results)
            {
                if (ct.IsCancellationRequested) break;
                await SyncTvSeriesAsync(info, ct);
            }

            await checkpoints.SaveAsync(SyncType, page, response.TotalPages, ct);
            logger.LogInformation("TV series: page {Page}/{Total}", page, response.TotalPages);

            if (page >= response.TotalPages) break;
        }

        await checkpoints.ResetAsync(SyncType, ct);
        logger.LogInformation("TV series sync complete");
    }

    private async Task SyncTvSeriesAsync(TVShowInfo info, CancellationToken ct)
    {
        try
        {
            await rateLimiter.ThrottleAsync(ct);
            var detail = await RetryHelper.ExecuteAsync(() => tvApi.FindByIdAsync(info.Id), ct: ct);
            if (detail.Item is null) return;

            var d = detail.Item;
            var runtime = d.EpisodeRunTime?.FirstOrDefault();
            var existing = await db.Movies.FirstOrDefaultAsync(m => m.IdTmdb == info.Id, ct);

            if (existing is null)
            {
                var show = Movie.Create(info.Id, info.Name, MovieType.Series);
                show.UpdateDetails(
                    info.Overview,
                    info.PosterPath,
                    info.BackdropPath,
                    info.FirstAirDate == default ? null : DateOnly.FromDateTime(info.FirstAirDate),
                    runtime is 0 ? null : runtime,
                    originalLanguage: d.OriginalLanguage,
                    numberOfSeasons: d.NumberOfSeasons,
                    numberOfEpisodes: d.NumberOfEpisodes);
                show.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
                show.UpdatePopularity(info.Popularity);
                db.Movies.Add(show);
                await db.SaveChangesAsync(ct);

                await SyncGenresAsync(show.Id, d.Genres.Select(g => (g.Id, g.Name)), ct);
                await SyncCreditsAsync(show.Id, info.Id, ct);
                await SyncProductionCompaniesAsync(show.Id, d.ProductionCompanies, ct);
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
                        info.FirstAirDate == default ? null : DateOnly.FromDateTime(info.FirstAirDate),
                        runtime is 0 ? null : runtime,
                        originalLanguage: d.OriginalLanguage,
                        numberOfSeasons: d.NumberOfSeasons,
                        numberOfEpisodes: d.NumberOfEpisodes);
                }

                await db.SaveChangesAsync(ct);
                await SyncGenresAsync(existing.Id, d.Genres.Select(g => (g.Id, g.Name)), ct);
                await SyncCreditsAsync(existing.Id, info.Id, ct);
                await SyncProductionCompaniesAsync(existing.Id, d.ProductionCompanies, ct);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to sync TV series IdTmdb={IdTmdb}", info.Id);
            db.ChangeTracker.Clear();
            await failures.RecordAsync(SyncType, info.Id, ex, ct);
        }
    }

    private async Task SyncGenresAsync(Guid movieId, IEnumerable<(int IdTmdb, string Name)> genres, CancellationToken ct)
    {
        var oldLinks = await db.MovieGenres.Where(mg => mg.MovieId == movieId).ToListAsync(ct);
        db.MovieGenres.RemoveRange(oldLinks);

        foreach (var g in genres)
        {
            var genreId = await EnsureGenreAsync(g.IdTmdb, g.Name, ct);
            db.MovieGenres.Add(new MovieGenre { MovieId = movieId, GenreId = genreId });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task SyncCreditsAsync(Guid movieId, int tmdbId, CancellationToken ct)
    {
        await rateLimiter.ThrottleAsync(ct);
        var credits = await RetryHelper.ExecuteAsync(() => directClient.GetTvCreditsAsync(tmdbId, ct), ct: ct);
        if (credits is null) return;

        var oldCast = await db.MovieCast.Where(c => c.MovieId == movieId).ToListAsync(ct);
        var oldCrew = await db.MovieCrew.Where(c => c.MovieId == movieId).ToListAsync(ct);
        db.MovieCast.RemoveRange(oldCast);
        db.MovieCrew.RemoveRange(oldCrew);

        foreach (var member in credits.Cast)
        {
            var personId = await EnsurePersonAsync(member.Id, ct);
            db.MovieCast.Add(new MovieCast
            {
                MovieId = movieId,
                PersonId = personId,
                Character = member.Character,
                Order = member.Order,
                CreditId = member.CreditId
            });
        }

        foreach (var member in credits.Crew)
        {
            var personId = await EnsurePersonAsync(member.Id, ct);
            db.MovieCrew.Add(new MovieCrew
            {
                MovieId = movieId,
                PersonId = personId,
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
            var companyId = await EnsureCompanyAsync(c.Id, c.Name, ct);
            db.MovieProductionCompanies.Add(new MovieProductionCompany { MovieId = movieId, CompanyId = companyId });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task<Guid> EnsurePersonAsync(int idTmdb, CancellationToken ct)
    {
        var tracked = db.ChangeTracker.Entries<Person>().FirstOrDefault(e => e.Entity.IdTmdb == idTmdb);
        if (tracked != null) return tracked.Entity.Id;

        var existing = await db.Persons.FirstOrDefaultAsync(p => p.IdTmdb == idTmdb, ct);
        if (existing != null) return existing.Id;

        var person = new Person { Id = Guid.NewGuid(), IdTmdb = idTmdb, Name = "Unknown", SyncedAt = DateTimeOffset.UtcNow };
        db.Persons.Add(person);
        return person.Id;
    }

    private async Task<Guid> EnsureGenreAsync(int idTmdb, string name, CancellationToken ct)
    {
        var tracked = db.ChangeTracker.Entries<Genre>().FirstOrDefault(e => e.Entity.IdTmdb == idTmdb);
        if (tracked != null) return tracked.Entity.Id;

        var existing = await db.Genres.FirstOrDefaultAsync(g => g.IdTmdb == idTmdb, ct);
        if (existing != null) return existing.Id;

        var genre = new Genre { Id = Guid.NewGuid(), IdTmdb = idTmdb, Name = name };
        db.Genres.Add(genre);
        return genre.Id;
    }

    private async Task<Guid> EnsureCompanyAsync(int idTmdb, string name, CancellationToken ct)
    {
        var tracked = db.ChangeTracker.Entries<ProductionCompany>().FirstOrDefault(e => e.Entity.IdTmdb == idTmdb);
        if (tracked != null) return tracked.Entity.Id;

        var existing = await db.ProductionCompanies.FirstOrDefaultAsync(c => c.IdTmdb == idTmdb, ct);
        if (existing != null) return existing.Id;

        var company = new ProductionCompany { Id = Guid.NewGuid(), IdTmdb = idTmdb, Name = name };
        db.ProductionCompanies.Add(company);
        return company.Id;
    }
}
