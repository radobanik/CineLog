using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Models.Profile;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class ProfileService(IUsersClient usersClient) : IProfileService
{
    public async Task<UserProfile> GetProfileAsync(CancellationToken ct = default)
    {
        var user = await usersClient.MeGETAsync(ct);
        return new UserProfile
        {
            Id = user.Id ?? Guid.Empty,
            Username = user.Username ?? string.Empty,
            Bio = user.Bio ?? string.Empty,
            AvatarUrl = user.AvatarUrl ?? string.Empty,
            FilmsCount = user.FilmsCount ?? 0,
            FollowersCount = user.FollowersCount ?? 0,
            FollowingCount = user.FollowingCount ?? 0
        };
    }

    public async Task<IReadOnlyList<MovieItem>> GetFavouriteMoviesAsync(CancellationToken ct = default)
    {
        var movies = await usersClient.FavoritesAllAsync(ct);
        return [.. movies.Select(m => new MovieItem
        {
            Id = m.Id ?? Guid.Empty,
            Title = m.Title ?? string.Empty,
            PosterPath = m.PosterPath,
            AverageRating = m.AverageRating
        })];
    }

    public async Task<IReadOnlyList<ReviewItem>> GetReviewsAsync(Guid userId, CancellationToken ct = default)
    {
        var response = await usersClient.ReviewsGET3Async(userId, null, null, ct);
        return [.. (response?.Items ?? []).Select(r => new ReviewItem
        {
            Id = r.Id ?? Guid.Empty,
            MovieTitle = r.MovieTitle ?? string.Empty,
            Rating = r.Rating,
            ReviewText = r.ReviewText,
            LikesCount = r.LikesCount ?? 0,
            CreatedAt = r.CreatedAt
        })];
    }
}
