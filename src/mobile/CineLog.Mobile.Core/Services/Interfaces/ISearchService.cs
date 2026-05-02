using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Search;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface ISearchService
{
    Task<(IReadOnlyList<MovieItem> Movies, bool HasMore)> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default);

    Task<(IReadOnlyList<UserSearchItem> Users, bool HasMore)> SearchUsersAsync(
        string query,
        int page,
        CancellationToken ct = default);
}
