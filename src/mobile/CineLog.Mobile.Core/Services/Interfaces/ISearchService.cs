using CineLog.Mobile.Core.Models.Home;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface ISearchService
{
    Task<IReadOnlyList<MovieItem>> SearchMoviesAsync(string query, CancellationToken ct = default);
}
