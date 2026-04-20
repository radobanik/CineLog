using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class SearchService(ISearchClient searchClient) : ISearchService
{
    public async Task<IReadOnlyList<MovieItem>> SearchMoviesAsync(string query, CancellationToken ct = default)
    {
        var result = await searchClient.SearchMoviesAsync(query, genres: null, page: 1, pageSize: 20, ct);

        return result.Items is null
            ? []
            : [.. result.Items.Select(m => new MovieItem
            {
                Id = m.Id ?? Guid.Empty,
                Title = m.Title ?? string.Empty,
                PosterPath = m.PosterPath,
                AverageRating = (double?)m.AverageRating
            })];
    }
}
