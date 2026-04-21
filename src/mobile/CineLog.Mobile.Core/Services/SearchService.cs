using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class SearchService(ISearchClient searchClient) : ISearchService
{
    private const int PageSize = 12;

    public async Task<(IReadOnlyList<MovieItem> Movies, bool HasMore)> SearchMoviesAsync(
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

        var hasMore = page < (result.TotalPages ?? 1);

        return (movies, hasMore);
    }
}
