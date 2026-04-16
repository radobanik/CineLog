using CineLog.Mobile.Core.Models.Home;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IHomeService
{
    Task<IReadOnlyList<MovieItem>> GetTopRatedMoviesAsync(int count, CancellationToken ct = default);
    Task<IReadOnlyList<MovieItem>> GetNewReleaseMoviesAsync(int count, CancellationToken ct = default);
}
