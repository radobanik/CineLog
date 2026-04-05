using CineLog.Domain.Enums;

namespace CineLog.Application.Features.Movies;

public record MovieListItemResponse(
    Guid Id,
    string Title,
    string? PosterPath,
    decimal AverageRating,
    MovieType Type,
    DateTimeOffset AddedAt);
