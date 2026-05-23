using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.Activity;
using CineLog.Mobile.Core.Services.Interfaces;
using ApiActivityFeedItem = CineLog.Mobile.ApiClient.Models.ActivityFeedItemResponse;
using ApiActivityType = CineLog.Mobile.ApiClient.Models.ActivityType;

namespace CineLog.Mobile.Core.Services;

public sealed class ActivityFeedService(IDashboardClient dashboardClient) : IActivityFeedService
{
    public async Task<IReadOnlyList<ActivityFeedItem>> GetActivityFeedAsync(
        int count = 50,
        CancellationToken ct = default)
    {
        var response = await dashboardClient.GetActivityFeedAsync(count, ct);
        return [.. response.Select(MapActivity)];
    }

    private static ActivityFeedItem MapActivity(ApiActivityFeedItem item)
    {
        return new ActivityFeedItem
        {
            Id = item.Id ?? Guid.Empty,
            Type = MapType(item.Type),
            CreatedAt = item.CreatedAt ?? DateTimeOffset.MinValue,

            ActorId = item.Actor?.Id ?? Guid.Empty,
            ActorUsername = item.Actor?.Username ?? "Unknown user",
            ActorAvatarUrl = item.Actor?.AvatarUrl,

            TargetUserId = item.TargetUser?.Id,
            TargetUsername = item.TargetUser?.Username,
            TargetAvatarUrl = item.TargetUser?.AvatarUrl,

            MovieId = item.Movie?.Id,
            MovieTitle = item.Movie?.Title,
            MoviePosterPath = item.Movie?.PosterPath,

            ReviewId = item.Review?.Id,
            ReviewText = item.Review?.ReviewText,
            ReviewRating = item.Review?.Rating,

            WatchlistId = item.Watchlist?.Id,
            WatchlistName = item.Watchlist?.Name
        };
    }

    private static ActivityFeedType MapType(ApiActivityType? type) => type switch
    {
        ApiActivityType._1 => ActivityFeedType.MovieWatched,
        ApiActivityType._2 => ActivityFeedType.MovieWatchLaterAdded,
        ApiActivityType._3 => ActivityFeedType.MovieFavorited,
        ApiActivityType._4 => ActivityFeedType.MovieAddedToCustomWatchlist,
        ApiActivityType._5 => ActivityFeedType.ReviewCreated,
        ApiActivityType._6 => ActivityFeedType.ReviewLiked,
        ApiActivityType._7 => ActivityFeedType.UserFollowed,
        ApiActivityType._8 => ActivityFeedType.MovieFavoriteRemoved,
        ApiActivityType._9 => ActivityFeedType.ReviewUnliked,
        ApiActivityType._10 => ActivityFeedType.UserUnfollowed,
        ApiActivityType._11 => ActivityFeedType.ProfileUpdated,
        ApiActivityType._12 => ActivityFeedType.AvatarUpdated,
        ApiActivityType._13 => ActivityFeedType.ReviewUpdated,
        ApiActivityType._14 => ActivityFeedType.ReviewDeleted,
        _ => ActivityFeedType.ProfileUpdated
    };
}
