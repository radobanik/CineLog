using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class SearchService(ISearchClient searchClient) : ISearchService
{
    public async Task<(IReadOnlyList<MovieItem> Movies, IReadOnlyList<PersonItem> People)> SearchAsync(
        string query,
        CancellationToken ct = default)
    {
        var result = await searchClient.SearchAsync(query, page: 1, pageSize: 20, ct);

        IReadOnlyList<MovieItem> movies = result.Movies is null
            ? []
            : [.. result.Movies.Select(m => new MovieItem
            {
                Id = m.Id ?? Guid.Empty,
                Title = m.Title ?? string.Empty,
                PosterPath = m.PosterPath,
                AverageRating = m.AverageRating
            })];

        IReadOnlyList<PersonItem> people = result.People is null
            ? []
            : [.. result.People.Select(p => new PersonItem
            {
                Id = p.Id ?? Guid.Empty,
                Name = p.Name ?? string.Empty,
                ProfilePath = p.ProfilePath
            })];

        return (movies, people);
    }
}
