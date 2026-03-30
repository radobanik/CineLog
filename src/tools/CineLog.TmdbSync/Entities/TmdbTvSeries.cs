namespace CineLog.TmdbSync.Entities;

public class TmdbTvSeries
{
    public int TmdbId { get; set; }
    public string Title { get; set; } = null!;
    public string? Overview { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public DateOnly? FirstAirDate { get; set; }
    public int? EpisodeRuntime { get; set; }
    public string? OriginalLanguage { get; set; }
    public string? Status { get; set; }
    public string? Tagline { get; set; }
    public int? NumberOfSeasons { get; set; }
    public int? NumberOfEpisodes { get; set; }
    public decimal AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public double Popularity { get; set; }
    public DateTimeOffset SyncedAt { get; set; }
}
