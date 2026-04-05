using MediatR;

namespace CineLog.Application.Features.People.UpdatePerson;

public record UpdatePersonCommand(
    Guid Id,
    string Name,
    string? ProfilePath,
    string? Biography,
    DateOnly? Birthday,
    string? PlaceOfBirth,
    double? Popularity) : IRequest;
