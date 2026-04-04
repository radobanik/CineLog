using MediatR;

namespace CineLog.Application.Features.Movies.UpdateMovie;

public record UpdateMovieCommand(
    Guid Id,
    string Title,
    string? Overview,
    string? PosterPath,
    string? BackdropPath,
    DateOnly? ReleaseDate,
    int? RuntimeMinutes,
    string? ImdbId,
    string? OriginalTitle,
    string? OriginalLanguage,
    string? Tagline,
    string? Status,
    long? Budget,
    decimal? Revenue,
    int? NumberOfSeasons,
    int? NumberOfEpisodes) : IRequest;
