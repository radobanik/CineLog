using CineLog.Mobile.Core.Models.Movies;
using CineLog.Mobile.Core.Models.Review;


namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IMovieDetailService
{
    Task<MovieDetailInfo> GetMovieDetailAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<ReviewListItem> Items, int TotalCount)> GetReviewsAsync(Guid movieId, int count, CancellationToken ct = default);
    Task<(IReadOnlyList<ReviewListItem> Items, int TotalCount, int TotalPages)> GetReviewsPageAsync(Guid movieId, int page, int pageSize, CancellationToken ct = default);
}
