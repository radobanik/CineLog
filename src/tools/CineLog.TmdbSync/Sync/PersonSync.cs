using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Infrastructure;
using DM.MovieApi.MovieDb.People;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Sync;

/// <summary>
/// Enriches person records that were added during cast/crew sync with full details from TMDb.
/// </summary>
public class PersonSync(
    TmdbSyncDbContext db,
    IApiPeopleRequest peopleApi,
    TmdbRateLimiter rateLimiter,
    FailureTracker failures,
    ILogger<PersonSync> logger)
{
    private const string SyncType = "persons";

    public async Task SyncAsync(CancellationToken ct)
    {
        var stalePersonIds = await db.Persons
            .Where(p => p.Biography == null)
            .OrderBy(p => p.SyncedAt)
            .Select(p => p.Id)
            .Take(500)
            .ToListAsync(ct);

        if (stalePersonIds.Count == 0)
        {
            logger.LogInformation("No persons to enrich");
            return;
        }

        logger.LogInformation("Enriching {Count} persons", stalePersonIds.Count);

        foreach (var personId in stalePersonIds)
        {
            if (ct.IsCancellationRequested) break;
            await EnrichPersonAsync(personId, ct);
        }

        logger.LogInformation("Person enrichment complete");
    }

    private async Task EnrichPersonAsync(int personId, CancellationToken ct)
    {
        try
        {
            await rateLimiter.ThrottleAsync(ct);
            var detail = await RetryHelper.ExecuteAsync(() => peopleApi.FindByIdAsync(personId), ct: ct);
            if (detail.Item is null) return;

            var d = detail.Item;
            var existing = await db.Persons.FindAsync([personId], ct);
            if (existing is null) return;

            existing.Name = d.Name;
            existing.ProfilePath = d.ProfilePath;
            existing.Biography = d.Biography?.Length > 5000 ? d.Biography[..5000] : d.Biography;
            existing.Birthday = d.Birthday == default ? null : DateOnly.FromDateTime(d.Birthday);
            existing.PlaceOfBirth = d.PlaceOfBirth;
            existing.Popularity = d.Popularity;
            existing.SyncedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to enrich person TmdbId={TmdbId}", personId);
            await failures.RecordAsync(SyncType, personId, ex, ct);
        }
    }
}
