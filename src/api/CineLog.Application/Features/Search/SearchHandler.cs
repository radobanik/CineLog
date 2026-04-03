using CineLog.Application.Common;
using CineLog.Application.Features.Movies;
using CineLog.Application.Features.People;
using CineLog.Domain.Enums;
using MediatR;

namespace CineLog.Application.Features.Search;

public class SearchHandler : IRequestHandler<SearchQuery, SearchResponse>
{
    private readonly IElasticSearchService _elasticSearch;

    public SearchHandler(IElasticSearchService elasticSearch) => _elasticSearch = elasticSearch;

    public async Task<SearchResponse> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        var moviesTask = _elasticSearch.SearchMoviesAsync(request.Query, request.Page, request.PageSize, cancellationToken);
        var peopleTask = _elasticSearch.SearchPeopleAsync(request.Query, request.Page, request.PageSize, cancellationToken);

        await Task.WhenAll(moviesTask, peopleTask);

        var movies = moviesTask.Result.Items
            .Select(d => new MovieSummaryResponse(
                Guid.Parse(d.Id), d.Title, d.PosterPath, d.AverageRating, Enum.Parse<MovieType>(d.Type)))
            .ToList();

        var people = peopleTask.Result.Items
            .Select(d => new PersonSummaryResponse(Guid.Parse(d.Id), d.Name, d.ProfilePath))
            .ToList();

        return new SearchResponse(movies, moviesTask.Result.TotalCount, people, peopleTask.Result.TotalCount);
    }
}
