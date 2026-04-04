using CineLog.Domain.Enums;
using MediatR;

namespace CineLog.Application.Features.Movies.CreateMovie;

public record CreateMovieCommand(
    int TmdbId,
    string Title,
    MovieType Type,
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
    int? NumberOfEpisodes) : IRequest<Guid>;
