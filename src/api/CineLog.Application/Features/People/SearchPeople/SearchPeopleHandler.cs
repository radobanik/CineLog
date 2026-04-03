using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.People.SearchPeople;

public class SearchPeopleHandler : IRequestHandler<SearchPeopleQuery, PagedResponse<PersonSummaryResponse>>
{
    private readonly IElasticSearchService _elasticSearch;

    public SearchPeopleHandler(IElasticSearchService elasticSearch)
        => _elasticSearch = elasticSearch;

    public async Task<PagedResponse<PersonSummaryResponse>> Handle(
        SearchPeopleQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _elasticSearch.SearchPeopleAsync(
            request.Query, request.Page, request.PageSize, cancellationToken);

        var items = result.Items
            .Select(d => new PersonSummaryResponse(Guid.Parse(d.Id), d.Name, d.ProfilePath))
            .ToList();

        return PagedResponse<PersonSummaryResponse>.Create(items, request.Page, request.PageSize, result.TotalCount);
    }
}
