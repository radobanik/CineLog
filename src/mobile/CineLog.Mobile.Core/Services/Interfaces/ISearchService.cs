using CineLog.Mobile.Core.Models.Home;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface ISearchService
{
    Task<(IReadOnlyList<MovieItem> Movies, bool HasMore)> SearchMoviesAsync(
        string query,
        int page,
        CancellationToken ct = default);
}
