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
    private readonly int _maxPages = configuration.GetValue("Sync:MaxDiscoverPages", 500);
    private readonly int _batchSize = configuration.GetValue("Sync:BatchSize", 5);

    public async Task SyncAsync(CancellationToken ct)
    {
        logger.LogInformation("Starting full TV series sync");
        var startPage = await checkpoints.GetLastPageAsync(SyncType, ct) + 1;

        for (var page = startPage; page <= _maxPages && !ct.IsCancellationRequested; page++)
        {
            await rateLimiter.ThrottleAsync(ct);
            var response = await RetryHelper.ExecuteAsync(
                () => tvApi.GetPopularAsync(page),
                ct: ct);

            if (!response.Results.Any()) break;

            var tasks = response.Results
                .Select(info => SyncTvSeriesAsync(info, ct))
                .Chunk(_batchSize)
                .Select(batch => Task.WhenAll(batch));

            foreach (var batch in tasks)
            {
                if (ct.IsCancellationRequested) break;
                await batch;
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
            var existing = await db.Movies.FirstOrDefaultAsync(m => m.TmdbId == info.Id, ct);

            if (existing is null)
            {
                var show = Movie.Create(info.Id, info.Name, MovieType.Series);
                show.UpdateDetails(
                    info.Overview,
                    info.PosterPath,
                    info.BackdropPath,
                    info.FirstAirDate == default ? null : DateOnly.FromDateTime(info.FirstAirDate),
                    runtime is 0 ? null : runtime,
                    d.Genres.Select(g => g.Name),
                    originalLanguage: d.OriginalLanguage,
                    numberOfSeasons: d.NumberOfSeasons,
                    numberOfEpisodes: d.NumberOfEpisodes);
                show.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
                show.UpdatePopularity(info.Popularity);
                db.Movies.Add(show);
                await db.SaveChangesAsync(ct);

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
                        d.Genres.Select(g => g.Name),
                        originalLanguage: d.OriginalLanguage,
                        numberOfSeasons: d.NumberOfSeasons,
                        numberOfEpisodes: d.NumberOfEpisodes);
                }

                await db.SaveChangesAsync(ct);
                await SyncCreditsAsync(existing.Id, info.Id, ct);
                await SyncProductionCompaniesAsync(existing.Id, d.ProductionCompanies, ct);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to sync TV series TmdbId={TmdbId}", info.Id);
            await failures.RecordAsync(SyncType, info.Id, ex, ct);
        }
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
            await EnsurePersonAsync(member.Id, ct);
            db.MovieCast.Add(new MovieCast
            {
                MovieId = movieId,
                PersonId = member.Id,
                Character = member.Character,
                Order = member.Order,
                CreditId = member.CreditId
            });
        }

        foreach (var member in credits.Crew)
        {
            await EnsurePersonAsync(member.Id, ct);
            db.MovieCrew.Add(new MovieCrew
            {
                MovieId = movieId,
                PersonId = member.Id,
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

    private async Task EnsurePersonAsync(int personId, CancellationToken ct)
    {
        if (!await db.Persons.AnyAsync(p => p.Id == personId, ct))
            db.Persons.Add(new Person { Id = personId, Name = "Unknown", SyncedAt = DateTimeOffset.UtcNow });
    }
}
