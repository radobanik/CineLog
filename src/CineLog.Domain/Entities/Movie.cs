using CineLog.Domain.Enums;

namespace CineLog.Domain.Entities;

public class Movie
{
    private List<string> _genres = [];
    private readonly List<Review> _reviews = [];

    public Guid Id { get; private set; }
    public int TmdbId { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Overview { get; private set; }
    public string? PosterPath { get; private set; }
    public string? BackdropPath { get; private set; }
    public DateOnly? ReleaseDate { get; private set; }
    public int? RuntimeMinutes { get; private set; }
    public MovieType Type { get; private set; }
    public decimal AverageRating { get; private set; }
    public int RatingsCount { get; private set; }

    public IReadOnlyCollection<string> Genres => _genres.AsReadOnly();

    private Movie() { }

    public static Movie Create(int tmdbId, string title, MovieType type)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            TmdbId = tmdbId,
            Title = title,
            Type = type
        };
    }

    public void UpdateDetails(
        string? overview,
        string? posterPath,
        string? backdropPath,
        DateOnly? releaseDate,
        int? runtimeMinutes,
        IEnumerable<string> genres)
    {
        Overview = overview;
        PosterPath = posterPath;
        BackdropPath = backdropPath;
        ReleaseDate = releaseDate;
        RuntimeMinutes = runtimeMinutes;
        _genres.Clear();
        _genres.AddRange(genres);
    }

    public void UpdateAverageRating(decimal averageRating, int ratingsCount)
    {
        AverageRating = averageRating;
        RatingsCount = ratingsCount;
    }
}
