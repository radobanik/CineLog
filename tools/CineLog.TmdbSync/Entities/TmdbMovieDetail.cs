namespace CineLog.TmdbSync.Entities;

public class TmdbMovieDetail
{
    public int TmdbId { get; set; }
    public string? ImdbId { get; set; }
    public string? OriginalTitle { get; set; }
    public string? OriginalLanguage { get; set; }
    public int Budget { get; set; }
    public decimal Revenue { get; set; }
    public string? Tagline { get; set; }
    public string? Status { get; set; }
    public double Popularity { get; set; }
    public DateTimeOffset SyncedAt { get; set; }
}
