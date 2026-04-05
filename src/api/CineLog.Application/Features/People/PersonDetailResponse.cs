namespace CineLog.Application.Features.People;

public record PersonDetailResponse(
    Guid Id,
    int IdTmdb,
    string Name,
    string? ProfilePath,
    string? Biography,
    DateOnly? Birthday,
    string? PlaceOfBirth,
    double? Popularity);
