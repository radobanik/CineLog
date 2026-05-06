using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class MovieSearchService(ISearchClient searchClient) : IMovieSearchService
{
    private const int PageSize = 12;

    public async Task<PagedResult<MovieItem>> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default)
    {
        var result = await searchClient.SearchMoviesAsync(query, genres: null, page, PageSize, ct);

        var movies = result.Items is null
            ? (IReadOnlyList<MovieItem>)[]
            : [.. result.Items.Select(m => new MovieItem
            {
                Id = m.Id ?? Guid.Empty,
                Title = m.Title ?? string.Empty,
                PosterPath = m.PosterPath,
                AverageRating = (double?)m.AverageRating
            })];

        return new PagedResult<MovieItem>(movies, page < (result.TotalPages ?? 1));
    }
}
