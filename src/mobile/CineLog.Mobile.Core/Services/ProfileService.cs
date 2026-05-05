using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Profile;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class ProfileService(IUsersClient usersClient) : IProfileService
{
    public async Task<UserProfile> GetProfileAsync(CancellationToken ct = default)
    {
        var user = await usersClient.GetMeAsync(ct);

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

    public async Task<UserProfile> GetProfileAsync(Guid? userId = null, CancellationToken ct = default)
    {
        var user = userId is { } id && id != Guid.Empty
            ? await usersClient.GetByIdAsync(id, ct)
            : await usersClient.GetMeAsync(ct);

        return new UserProfile
        {
            Id = user.Id ?? Guid.Empty,
            Username = user.Username ?? string.Empty,
            Bio = user.Bio ?? string.Empty,
            AvatarUrl = user.AvatarUrl ?? string.Empty,
            FilmsCount = user.FilmsCount ?? 0,
            FollowersCount = user.FollowersCount ?? 0,
            FollowingCount = user.FollowingCount ?? 0,
            IsFollowing = user.IsFollowing ?? false
        };
    }


    public async Task<IReadOnlyList<MovieItem>> GetFavouriteMoviesAsync(CancellationToken ct = default)
    {
        var movies = await usersClient.GetFavoritesAsync(ct);
        return [.. movies.Select(m => new MovieItem
        {
            Id = m.Id ?? Guid.Empty,
            Title = m.Title ?? string.Empty,
            PosterPath = m.PosterPath,
            AverageRating = m.AverageRating
        })];
    }

    public async Task<IReadOnlyList<ReviewListItem>> GetReviewsAsync(Guid userId, CancellationToken ct = default)
    {
        var response = await usersClient.GetReviewsAsync(userId, null, null, ct);
        return [.. (response?.Items ?? []).Select(r => new ReviewListItem
        {
            Id = r.Id ?? Guid.Empty,
            MovieTitle = r.MovieTitle ?? string.Empty,
            Rating = r.Rating,
            ReviewText = r.ReviewText,
            LikesCount = r.LikesCount ?? 0,
            CreatedAt = r.CreatedAt
        })];
    }

    public async Task<(IReadOnlyList<ReviewListItem> Items, int TotalCount, int TotalPages)> GetReviewsPageAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var response = await usersClient.GetReviewsAsync(userId, page, pageSize, ct);
        var items = (response?.Items ?? []).Select(r => new ReviewListItem
        {
            Id = r.Id ?? Guid.Empty,
            MovieTitle = r.MovieTitle ?? string.Empty,
            Rating = r.Rating,
            ReviewText = r.ReviewText,
            LikesCount = r.LikesCount ?? 0,
            CreatedAt = r.CreatedAt,
        }).ToList();
        return (items, response?.TotalCount ?? 0, response?.TotalPages ?? 0);
    }
}
