using CineLog.Domain.Enums;
using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Entities;
using CineLog.TmdbSync.Infrastructure;
using DM.MovieApi.MovieDb.TV;
using Microsoft.EntityFrameworkCore;
using DomainMovie = CineLog.Domain.Entities.Movie;

namespace CineLog.TmdbSync.Sync;

public class TvSeriesFullSync(
    TmdbSyncDbContext appDb,
    TmdbSchemaDbContext tmdbDb,
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
            await SyncPublicMovieAsync(info, ct);
            await SyncTvDetailsAsync(info, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to sync TV series TmdbId={TmdbId}", info.Id);
            await failures.RecordAsync(SyncType, info.Id, ex, ct);
        }
    }

    private async Task SyncPublicMovieAsync(TVShowInfo info, CancellationToken ct)
    {
        var existing = await appDb.Movies.FirstOrDefaultAsync(m => m.TmdbId == info.Id, ct);

        if (existing is null)
        {
            await rateLimiter.ThrottleAsync(ct);
            var detail = await RetryHelper.ExecuteAsync(() => tvApi.FindByIdAsync(info.Id), ct: ct);

            var show = DomainMovie.Create(info.Id, info.Name, MovieType.Series);
            var runtime = detail.Item?.EpisodeRunTime?.FirstOrDefault();
            show.UpdateDetails(
                info.Overview,
                info.PosterPath,
                info.BackdropPath,
                info.FirstAirDate == default ? null : DateOnly.FromDateTime(info.FirstAirDate),
                runtime is 0 ? null : runtime,
                detail.Item?.Genres.Select(g => g.Name).ToList() ?? []);
            show.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
            appDb.Movies.Add(show);
        }
        else
        {
            existing.UpdateAverageRating((decimal)info.VoteAverage, info.VoteCount);
        }

        await appDb.SaveChangesAsync(ct);
    }

    private async Task SyncTvDetailsAsync(TVShowInfo info, CancellationToken ct)
    {
        await rateLimiter.ThrottleAsync(ct);
        var detail = await RetryHelper.ExecuteAsync(() => tvApi.FindByIdAsync(info.Id), ct: ct);
        if (detail.Item is null) return;

        var d = detail.Item;
        var runtime = d.EpisodeRunTime?.FirstOrDefault();

        var existing = await tmdbDb.TvSeries.FindAsync([info.Id], ct);
        if (existing is null)
        {
            tmdbDb.TvSeries.Add(new TmdbTvSeries
            {
                TmdbId = info.Id,
                Title = info.Name,
                Overview = info.Overview,
                PosterPath = info.PosterPath,
                BackdropPath = info.BackdropPath,
                FirstAirDate = info.FirstAirDate == default ? null : DateOnly.FromDateTime(info.FirstAirDate),
                EpisodeRuntime = runtime is 0 ? null : runtime,
                OriginalLanguage = d.OriginalLanguage,
                NumberOfSeasons = d.NumberOfSeasons,
                NumberOfEpisodes = d.NumberOfEpisodes,
                AverageRating = (decimal)info.VoteAverage,
                RatingsCount = info.VoteCount,
                Popularity = info.Popularity,
                SyncedAt = DateTimeOffset.UtcNow
            });
        }
        else
        {
            existing.AverageRating = (decimal)info.VoteAverage;
            existing.RatingsCount = info.VoteCount;
            existing.Popularity = info.Popularity;
            existing.NumberOfSeasons = d.NumberOfSeasons;
            existing.NumberOfEpisodes = d.NumberOfEpisodes;
            existing.SyncedAt = DateTimeOffset.UtcNow;
        }

        await SyncTvGenresAsync(info.Id, d.Genres.Select(g => g.Id).ToList(), ct);
        await SyncTvProductionCompaniesAsync(info.Id, d.ProductionCompanies, ct);
        await SyncTvCreditsAsync(info.Id, ct);

        await tmdbDb.SaveChangesAsync(ct);
    }

    private async Task SyncTvGenresAsync(int tmdbId, List<int> genreIds, CancellationToken ct)
    {
        var existingLinks = await tmdbDb.TvGenres.Where(g => g.TmdbTvId == tmdbId).ToListAsync(ct);
        tmdbDb.TvGenres.RemoveRange(existingLinks);

        foreach (var gid in genreIds)
        {
            if (!await tmdbDb.Genres.AnyAsync(g => g.Id == gid, ct))
                tmdbDb.Genres.Add(new TmdbGenre { Id = gid, Name = "Unknown", IsTvGenre = true });
            tmdbDb.TvGenres.Add(new TmdbTvGenre { TmdbTvId = tmdbId, GenreId = gid });
        }
    }

    private async Task SyncTvProductionCompaniesAsync(
        int tmdbId,
        IEnumerable<DM.MovieApi.MovieDb.Companies.ProductionCompanyInfo> companies,
        CancellationToken ct)
    {
        var existingLinks = await tmdbDb.TvProductionCompanies.Where(tp => tp.TmdbTvId == tmdbId).ToListAsync(ct);
        tmdbDb.TvProductionCompanies.RemoveRange(existingLinks);

        foreach (var c in companies)
        {
            if (!await tmdbDb.ProductionCompanies.AnyAsync(pc => pc.Id == c.Id, ct))
                tmdbDb.ProductionCompanies.Add(new TmdbProductionCompany { Id = c.Id, Name = c.Name });
            tmdbDb.TvProductionCompanies.Add(new TmdbTvProductionCompany { TmdbTvId = tmdbId, CompanyId = c.Id });
        }
    }

    private async Task SyncTvCreditsAsync(int tmdbId, CancellationToken ct)
    {
        await rateLimiter.ThrottleAsync(ct);
        var credits = await RetryHelper.ExecuteAsync(() => directClient.GetTvCreditsAsync(tmdbId, ct), ct: ct);
        if (credits is null) return;

        var oldCast = await tmdbDb.TvCast.Where(c => c.TmdbTvId == tmdbId).ToListAsync(ct);
        var oldCrew = await tmdbDb.TvCrew.Where(c => c.TmdbTvId == tmdbId).ToListAsync(ct);
        tmdbDb.TvCast.RemoveRange(oldCast);
        tmdbDb.TvCrew.RemoveRange(oldCrew);

        foreach (var member in credits.Cast)
        {
            await EnsurePersonAsync(member.Id, ct);
            tmdbDb.TvCast.Add(new TmdbTvCast
            {
                TmdbTvId = tmdbId,
                PersonId = member.Id,
                Character = member.Character,
                Order = member.Order,
                CreditId = member.CreditId
            });
        }

        foreach (var member in credits.Crew)
        {
            await EnsurePersonAsync(member.Id, ct);
            tmdbDb.TvCrew.Add(new TmdbTvCrew
            {
                TmdbTvId = tmdbId,
                PersonId = member.Id,
                Department = member.Department,
                Job = member.Job,
                CreditId = member.CreditId
            });
        }
    }

    private async Task EnsurePersonAsync(int personId, CancellationToken ct)
    {
        if (!await tmdbDb.Persons.AnyAsync(p => p.Id == personId, ct))
            tmdbDb.Persons.Add(new TmdbPerson { Id = personId, Name = "Unknown", SyncedAt = DateTimeOffset.UtcNow });
    }
}
