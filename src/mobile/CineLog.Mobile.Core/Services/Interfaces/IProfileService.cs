using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Profile;
using CineLog.Mobile.Core.Models.Review;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IProfileService
{
    Task<UserProfile> GetProfileAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MovieItem>> GetFavouriteMoviesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ReviewItem>> GetReviewsAsync(Guid userId, CancellationToken ct = default);
}
