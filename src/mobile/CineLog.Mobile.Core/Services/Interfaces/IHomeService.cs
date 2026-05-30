using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Review;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IHomeService
{
    Task<IReadOnlyList<MovieItem>> GetTopRatedMoviesAsync(int count, CancellationToken ct = default);
    Task<IReadOnlyList<MovieItem>> GetNewReleaseMoviesAsync(int count, CancellationToken ct = default);
    Task<IReadOnlyList<ReviewListItem>> GetLatestReviewsAsync(int count, CancellationToken ct = default);
}
