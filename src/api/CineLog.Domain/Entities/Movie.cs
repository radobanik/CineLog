using CineLog.Domain.Enums;

namespace CineLog.Domain.Entities;

public class Movie
{
    private List<string> _genres = [];
    private readonly List<Review> _reviews = [];
    private readonly List<MovieCast> _cast = [];
    private readonly List<MovieCrew> _crew = [];
    private readonly List<MovieProductionCompany> _productionCompanies = [];

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

    // Extended fields
    public string? ImdbId { get; private set; }
    public string? OriginalTitle { get; private set; }
    public string? OriginalLanguage { get; private set; }
    public string? Tagline { get; private set; }
    public string? Status { get; private set; }
    public long? Budget { get; private set; }
    public decimal? Revenue { get; private set; }
    public double? Popularity { get; private set; }
    public int? NumberOfSeasons { get; private set; }
    public int? NumberOfEpisodes { get; private set; }

    public bool IsManuallyEdited { get; private set; }

    public IReadOnlyCollection<string> Genres => _genres.AsReadOnly();
    public IReadOnlyCollection<MovieCast> Cast => _cast.AsReadOnly();
    public IReadOnlyCollection<MovieCrew> Crew => _crew.AsReadOnly();
    public IReadOnlyCollection<MovieProductionCompany> ProductionCompanies => _productionCompanies.AsReadOnly();

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
        IEnumerable<string> genres,
        string? imdbId = null,
        string? originalTitle = null,
        string? originalLanguage = null,
        string? tagline = null,
        string? status = null,
        long? budget = null,
        decimal? revenue = null,
        int? numberOfSeasons = null,
        int? numberOfEpisodes = null)
    {
        Overview = overview;
        PosterPath = posterPath;
        BackdropPath = backdropPath;
        ReleaseDate = releaseDate;
        RuntimeMinutes = runtimeMinutes;
        _genres.Clear();
        _genres.AddRange(genres);
        ImdbId = imdbId;
        OriginalTitle = originalTitle;
        OriginalLanguage = originalLanguage;
        Tagline = tagline;
        Status = status;
        Budget = budget;
        Revenue = revenue;
        NumberOfSeasons = numberOfSeasons;
        NumberOfEpisodes = numberOfEpisodes;
    }

    public void UpdateAverageRating(decimal averageRating, int ratingsCount)
    {
        AverageRating = averageRating;
        RatingsCount = ratingsCount;
    }

    public void UpdatePopularity(double? popularity)
    {
        Popularity = popularity;
    }

    public void MarkAsManuallyEdited()
    {
        IsManuallyEdited = true;
    }
}
