namespace CineLog.Application.Common;

public record MovieSearchDocument(
    string Id,
    int TmdbId,
    string Title,
    string? OriginalTitle,
    string? Overview,
    string Type,
    List<string> Genres,
    string? PosterPath,
    string? ReleaseDate,
    decimal AverageRating,
    int RatingsCount);
