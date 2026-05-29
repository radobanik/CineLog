using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Activity;
using CineLog.Mobile.Core.Services.Interfaces;
using Newtonsoft.Json;
using ApiActivityFeedItem = CineLog.Mobile.ApiClient.Models.ActivityFeedItemResponse;
using ApiActivityType = CineLog.Mobile.ApiClient.Models.ActivityType;

namespace CineLog.Mobile.Core.Services;

public sealed class ActivityFeedService(
    IHttpClientFactory httpClientFactory,
    ISessionService session) : IActivityFeedService
{
    public async Task<IReadOnlyList<ActivityFeedItem>> GetActivityFeedAsync(
        int skip = 0,
        int count = 25,
        CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("CineLogApi");
        var response = await client.GetAsync($"api/dashboard/activity-feed?skip={skip}&count={count}", ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var items = JsonConvert.DeserializeObject<ICollection<ApiActivityFeedItem>>(json) ?? [];

        return [.. items.Select(MapActivity)];
    }

    private ActivityFeedItem MapActivity(ApiActivityFeedItem item)
    {
        var actorId = item.Actor?.Id ?? Guid.Empty;
        var targetUserId = item.TargetUser?.Id;

        return new ActivityFeedItem
        {
            Id = item.Id ?? Guid.Empty,
            Type = MapType(item.Type),
            CreatedAt = item.CreatedAt ?? DateTimeOffset.MinValue,

            ActorId = actorId,
            ActorUsername = item.Actor?.Username ?? "Unknown user",
            ActorAvatarUrl = item.Actor?.AvatarUrl,
            IsCurrentUser = actorId == session.UserId,

            TargetUserId = targetUserId,
            TargetUsername = item.TargetUser?.Username,
            TargetAvatarUrl = item.TargetUser?.AvatarUrl,
            IsTargetCurrentUser = targetUserId == session.UserId,

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
