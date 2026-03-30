using CineLog.Domain.Enums;

namespace CineLog.Application.Features.Movies;

public record MovieSummaryResponse(
    Guid Id,
    string Title,
    string? PosterPath,
    decimal AverageRating,
    MovieType Type);
