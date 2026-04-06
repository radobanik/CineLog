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
    ILogger<PersonSync> logger,
    IConfiguration configuration)
{
    private const string SyncType = "persons";
    private readonly int _maxPersons = configuration.GetValue("Sync:PersonMaxDiscoverPages", 500);

    public async Task SyncAsync(CancellationToken ct)
    {
        var query = db.Persons
            .Where(p => p.Biography == null)
            .OrderBy(p => p.SyncedAt)
            .Select(p => new { p.Id, p.IdTmdb });

        var stalePersons = await (_maxPersons == -1 ? query : query.Take(_maxPersons))
            .ToListAsync(ct);

        if (stalePersons.Count == 0)
        {
            logger.LogInformation("No persons to enrich");
            return;
        }

        logger.LogInformation("Enriching {Count} persons", stalePersons.Count);

        foreach (var person in stalePersons)
        {
            if (ct.IsCancellationRequested) break;
            await EnrichPersonAsync(person.Id, person.IdTmdb, ct);
        }

        logger.LogInformation("Person enrichment complete");
    }

    private async Task EnrichPersonAsync(Guid id, int idTmdb, CancellationToken ct)
    {
        try
        {
            await rateLimiter.ThrottleAsync(ct);
            var detail = await RetryHelper.ExecuteAsync(() => peopleApi.FindByIdAsync(idTmdb), ct: ct);
            if (detail.Item is null) return;

            var d = detail.Item;
            var existing = await db.Persons.FindAsync([id], ct);
            if (existing is null) return;

            existing.Name = d.Name;
            existing.ProfilePath = TmdbImage.Url(d.ProfilePath);
            existing.Biography = d.Biography?.Length > 5000 ? d.Biography[..5000] : d.Biography;
            existing.Birthday = d.Birthday == default ? null : DateOnly.FromDateTime(d.Birthday);
            existing.PlaceOfBirth = d.PlaceOfBirth;
            existing.Popularity = d.Popularity;
            existing.SyncedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Failed to enrich person IdTmdb={IdTmdb}", idTmdb);
            await failures.RecordAsync(SyncType, idTmdb, ex, ct);
        }
    }
}
