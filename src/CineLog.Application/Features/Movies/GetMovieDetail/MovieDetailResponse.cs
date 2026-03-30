namespace CineLog.Application.Features.Movies.GetMovieDetail;

public record MovieDetailResponse(
    Guid Id,
    int TmdbId,
    string Title,
    string? Overview,
    string? PosterPath,
    decimal AverageRating,
    int RatingsCount,
    List<string> Genres,
    int? RuntimeMinutes);
