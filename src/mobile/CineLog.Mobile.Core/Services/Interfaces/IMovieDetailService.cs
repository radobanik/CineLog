using CineLog.Mobile.Core.Models.Movies;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IMovieDetailService
{
    Task<MovieDetailInfo> GetMovieDetailAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<ReviewPreviewItem> Items, int TotalCount)> GetReviewsAsync(Guid movieId, int count, CancellationToken ct = default);
    Task<(IReadOnlyList<ReviewPreviewItem> Items, int TotalCount, int TotalPages)> GetReviewsPageAsync(Guid movieId, int page, int pageSize, CancellationToken ct = default);
}
