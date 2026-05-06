using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Search;

namespace CineLog.Mobile.Core.Services.Interfaces;

// User search lives in IUserService — this interface owns movie search only.
public interface IMovieSearchService
{
    Task<PagedResult<MovieItem>> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default);
}
