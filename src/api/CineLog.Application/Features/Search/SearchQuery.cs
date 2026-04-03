using MediatR;

namespace CineLog.Application.Features.Search;

public record SearchQuery(string Query, int Page = 1, int PageSize = 20) : IRequest<SearchResponse>;
