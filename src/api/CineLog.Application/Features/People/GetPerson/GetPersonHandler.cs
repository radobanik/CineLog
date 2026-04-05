using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.People.GetPerson;

public class GetPersonHandler : IRequestHandler<GetPersonQuery, PersonDetailResponse>
{
    private readonly IPersonRepository _personRepository;

    public GetPersonHandler(IPersonRepository personRepository)
        => _personRepository = personRepository;

    public async Task<PersonDetailResponse> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken)
            ?? throw new NotFoundException($"Person {request.PersonId} not found.");

        return new PersonDetailResponse(
            person.Id,
            person.IdTmdb,
            person.Name,
            person.ProfilePath,
            person.Biography,
            person.Birthday,
            person.PlaceOfBirth,
            person.Popularity);
    }
}
