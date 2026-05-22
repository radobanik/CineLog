using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Search;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IMovieSearchService
{
    Task<PagedResult<MovieItem>> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default);

    Task<PagedResult<MovieItem>> SearchMoviesByCategoryAsync(
        MovieCategory category,
        int page,
        int pageSize,
        CancellationToken ct = default);
}
