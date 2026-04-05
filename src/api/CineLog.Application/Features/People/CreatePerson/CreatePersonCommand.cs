using MediatR;

namespace CineLog.Application.Features.People.CreatePerson;

public record CreatePersonCommand(
    int IdTmdb,
    string Name,
    string? ProfilePath,
    string? Biography,
    DateOnly? Birthday,
    string? PlaceOfBirth,
    double? Popularity) : IRequest<Guid>;
