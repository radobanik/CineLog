using CineLog.TmdbSync.Data;
using CineLog.TmdbSync.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineLog.TmdbSync.Infrastructure;

public class CheckpointService(TmdbSyncDbContext db)
{
    public async Task<int> GetLastPageAsync(string syncType, CancellationToken ct = default)
    {
        var checkpoint = await db.SyncCheckpoints.FindAsync([syncType], ct);
        return checkpoint?.LastPage ?? 0;
    }

    public async Task SaveAsync(string syncType, int page, int totalPages, CancellationToken ct = default)
    {
        var checkpoint = await db.SyncCheckpoints.FindAsync([syncType], ct);
        if (checkpoint is null)
        {
            checkpoint = new SyncCheckpoint { SyncType = syncType };
            db.SyncCheckpoints.Add(checkpoint);
        }

        checkpoint.LastPage = page;
        checkpoint.TotalPages = totalPages;
        checkpoint.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
    }

    public async Task ResetAsync(string syncType, CancellationToken ct = default)
    {
        var checkpoint = await db.SyncCheckpoints.FindAsync([syncType], ct);
        if (checkpoint is not null)
        {
            db.SyncCheckpoints.Remove(checkpoint);
            await db.SaveChangesAsync(ct);
        }
    }
}
