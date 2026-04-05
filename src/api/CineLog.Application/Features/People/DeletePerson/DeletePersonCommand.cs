using MediatR;

namespace CineLog.Application.Features.People.DeletePerson;

public record DeletePersonCommand(Guid PersonId) : IRequest;
