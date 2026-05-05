using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class MovieSearchService(ISearchClient searchClient) : IMovieSearchService
{
    private const int SearchPageSize = 12;

    public Task<PagedResult<MovieItem>> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default) =>
        SearchAsync(query, genres: null, page, SearchPageSize, ct);

    public Task<PagedResult<MovieItem>> SearchMoviesByCategoryAsync(
    MovieCategory category,
    int page,
    int pageSize,
    CancellationToken ct = default)
    {
        var genre = ToGenreName(category);

        return genre is null
            ? SearchAsync(" ", genres: null, page, pageSize, ct)
            : SearchAsync(" ", [genre], page, pageSize, ct);
    }
    private async Task<PagedResult<MovieItem>> SearchAsync(
        string query,
        IEnumerable<string>? genres,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var result = await searchClient.SearchMoviesAsync(
            query,
            genres,
            page,
            pageSize,
            ct);

        var movies = result.Items is null
            ? (IReadOnlyList<MovieItem>)[]
            : [.. result.Items.Select(movie => new MovieItem
            {
                Id = movie.Id ?? Guid.Empty,
                Title = movie.Title ?? string.Empty,
                PosterPath = movie.PosterPath,
                AverageRating = movie.AverageRating
            })];

        return new PagedResult<MovieItem>(
            movies,
            page < (result.TotalPages ?? 1));
    }

    private static string? ToGenreName(MovieCategory category) => category switch
    {
        MovieCategory.Action => "Action",
        MovieCategory.Drama => "Drama",
        MovieCategory.Scifi => "Science Fiction",
        MovieCategory.Horror => "Horror",
        _ => null
    };
}
