using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Entities;

namespace CineLog.TmdbSync.Infrastructure;

public class FailureTracker(TmdbSyncDbContext db)
{
    public async Task RecordAsync(string syncType, int tmdbId, Exception ex, CancellationToken ct = default)
    {
        db.SyncFailures.Add(new SyncFailure
        {
            SyncType = syncType,
            TmdbId = tmdbId,
            ErrorMessage = ex.Message.Length > 2000 ? ex.Message[..2000] : ex.Message,
            RetryCount = 0,
            FailedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync(ct);
    }
}
