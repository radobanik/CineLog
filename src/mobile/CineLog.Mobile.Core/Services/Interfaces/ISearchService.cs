using CineLog.Mobile.Core.Models;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface ISearchService
{
    Task<(IReadOnlyList<MovieItem> Movies, bool HasMore)> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default);
}
