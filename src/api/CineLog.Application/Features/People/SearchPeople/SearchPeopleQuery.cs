using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.People.SearchPeople;

public record SearchPeopleQuery(string Query, int Page = 1, int PageSize = 20)
    : IRequest<PagedResponse<PersonSummaryResponse>>;
