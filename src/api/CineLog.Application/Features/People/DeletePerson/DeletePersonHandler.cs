using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.People.DeletePerson;

public class DeletePersonHandler : IRequestHandler<DeletePersonCommand>
{
    private readonly IPersonRepository _personRepository;
    private readonly IElasticSearchService _elasticSearch;

    public DeletePersonHandler(IPersonRepository personRepository, IElasticSearchService elasticSearch)
    {
        _personRepository = personRepository;
        _elasticSearch = elasticSearch;
    }

    public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken)
            ?? throw new NotFoundException($"Person {request.PersonId} not found.");

        await _personRepository.DeleteAsync(person, cancellationToken);
        await _elasticSearch.DeletePersonAsync(request.PersonId, cancellationToken);
    }
}
