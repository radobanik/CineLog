using CineLog.Domain.Enums;

namespace CineLog.Application.Features.Movies;

public record MovieSearchData(
    int TmdbId,
    string Title,
    string? Overview,
    string? PosterPath,
    string? BackdropPath,
    DateOnly? ReleaseDate,
    int? RuntimeMinutes,
    IReadOnlyCollection<string> Genres,
    MovieType Type);
