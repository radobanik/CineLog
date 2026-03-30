namespace CineLog.TmdbSync.Entities;

public class SyncFailure
{
    public long Id { get; set; }
    public string SyncType { get; set; } = null!;
    public int TmdbId { get; set; }
    public string ErrorMessage { get; set; } = null!;
    public int RetryCount { get; set; }
    public DateTimeOffset FailedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
}
