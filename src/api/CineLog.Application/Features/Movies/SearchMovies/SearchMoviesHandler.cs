using CineLog.Application.Common;
using CineLog.Domain.Enums;
using MediatR;

namespace CineLog.Application.Features.Movies.SearchMovies;

public class SearchMoviesHandler : IRequestHandler<SearchMoviesQuery, PagedResponse<MovieSummaryResponse>>
{
    private readonly IElasticSearchService _elasticSearch;

    public SearchMoviesHandler(IElasticSearchService elasticSearch)
        => _elasticSearch = elasticSearch;

    public async Task<PagedResponse<MovieSummaryResponse>> Handle(
        SearchMoviesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _elasticSearch.SearchMoviesAsync(
            request.Query, request.Page, request.PageSize, cancellationToken);

        var items = result.Items
            .Select(d => new MovieSummaryResponse(
                Guid.Parse(d.Id),
                d.Title,
                d.PosterPath,
                d.AverageRating,
                Enum.Parse<MovieType>(d.Type)))
            .ToList();

        return PagedResponse<MovieSummaryResponse>.Create(items, request.Page, request.PageSize, result.TotalCount);
    }
}
