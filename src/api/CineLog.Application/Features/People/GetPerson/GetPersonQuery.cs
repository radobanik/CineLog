using MediatR;

namespace CineLog.Application.Features.People.GetPerson;

public record GetPersonQuery(Guid PersonId) : IRequest<PersonDetailResponse>;
