using CineLog.Domain.Enums;

namespace CineLog.Infrastructure.ExternalApis;

public record TmdbMovieResult(
    int TmdbId,
    string Title,
    string? Overview,
    string? PosterPath,
    string? BackdropPath,
    DateOnly? ReleaseDate,
    int? Runtime,
    IReadOnlyCollection<string> Genres,
    MovieType Type
);
