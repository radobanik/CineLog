using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.People.UpdatePerson;

public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand>
{
    private readonly IPersonRepository _personRepository;
    private readonly IElasticSearchService _elasticSearch;

    public UpdatePersonHandler(IPersonRepository personRepository, IElasticSearchService elasticSearch)
    {
        _personRepository = personRepository;
        _elasticSearch = elasticSearch;
    }

    public async Task Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Person {request.Id} not found.");

        person.Name = request.Name;
        person.ProfilePath = request.ProfilePath;
        person.Biography = request.Biography;
        person.Birthday = request.Birthday;
        person.PlaceOfBirth = request.PlaceOfBirth;
        person.Popularity = request.Popularity;
        person.SyncedAt = DateTimeOffset.UtcNow;

        await _personRepository.UpdateAsync(person, cancellationToken);
        await _elasticSearch.IndexPersonAsync(
            new PersonSearchDocument(person.Id.ToString(), person.Name, person.ProfilePath),
            cancellationToken);
    }
}
