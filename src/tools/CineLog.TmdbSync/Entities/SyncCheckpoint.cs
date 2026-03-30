namespace CineLog.TmdbSync.Entities;

public class SyncCheckpoint
{
    public string SyncType { get; set; } = null!;
    public int LastPage { get; set; }
    public int TotalPages { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
