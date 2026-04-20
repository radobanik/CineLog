using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Models.Search;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface ISearchService
{
    Task<(IReadOnlyList<MovieItem> Movies, IReadOnlyList<PersonItem> People)> SearchAsync(
        string query,
        CancellationToken ct = default);
}
