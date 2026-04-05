using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.People.CreatePerson;

public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    private readonly IPersonRepository _personRepository;
    private readonly IElasticSearchService _elasticSearch;

    public CreatePersonHandler(IPersonRepository personRepository, IElasticSearchService elasticSearch)
    {
        _personRepository = personRepository;
        _elasticSearch = elasticSearch;
    }

    public async Task<Guid> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            IdTmdb = request.IdTmdb,
            Name = request.Name,
            ProfilePath = request.ProfilePath,
            Biography = request.Biography,
            Birthday = request.Birthday,
            PlaceOfBirth = request.PlaceOfBirth,
            Popularity = request.Popularity,
            SyncedAt = DateTimeOffset.UtcNow
        };

        await _personRepository.AddAsync(person, cancellationToken);
        await _elasticSearch.IndexPersonAsync(
            new PersonSearchDocument(person.Id.ToString(), person.Name, person.ProfilePath),
            cancellationToken);

        return person.Id;
    }
}
