namespace CineLog.Mobile.Core.Models.Activity;

public sealed class ActivityFeedItem
{
    public Guid Id { get; init; }
    public ActivityFeedType Type { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public Guid ActorId { get; init; }
    public string ActorUsername { get; init; } = string.Empty;
    public string? ActorAvatarUrl { get; init; }

    public Guid? TargetUserId { get; init; }
    public string? TargetUsername { get; init; }
    public string? TargetAvatarUrl { get; init; }

    public Guid? MovieId { get; init; }
    public string? MovieTitle { get; init; }
    public string? MoviePosterPath { get; init; }

    public Guid? ReviewId { get; init; }
    public string? ReviewText { get; init; }
    public double? ReviewRating { get; init; }

    public Guid? WatchlistId { get; init; }
    public string? WatchlistName { get; init; }

    public string ActorInitial =>
        string.IsNullOrWhiteSpace(ActorUsername)
            ? "?"
            : ActorUsername[..1].ToUpperInvariant();

    public bool HasActorAvatar => !string.IsNullOrWhiteSpace(ActorAvatarUrl);
    public bool HasMovie => MovieId.HasValue;
    public bool HasMoviePoster => !string.IsNullOrWhiteSpace(MoviePosterPath);
    public bool HasReviewText => !string.IsNullOrWhiteSpace(ReviewText);
    public bool HasTargetUser => TargetUserId.HasValue;
    public bool HasWatchlist => WatchlistId.HasValue;

    public string ActionText => Type switch
    {
        ActivityFeedType.MovieWatched =>
            $"{ActorUsername} watched {MovieLabel}",

        ActivityFeedType.MovieWatchLaterAdded =>
            $"{ActorUsername} added {MovieLabel} to Watch later",

        ActivityFeedType.MovieFavorited =>
            $"{ActorUsername} added {MovieLabel} to Favorites",

        ActivityFeedType.MovieFavoriteRemoved =>
            $"{ActorUsername} removed {MovieLabel} from Favorites",

        ActivityFeedType.MovieAddedToCustomWatchlist =>
            $"{ActorUsername} added {MovieLabel} to {WatchlistLabel}",

        ActivityFeedType.ReviewCreated =>
            $"{ActorUsername} reviewed {MovieLabel}",

        ActivityFeedType.ReviewUpdated =>
            $"{ActorUsername} updated a review for {MovieLabel}",

        ActivityFeedType.ReviewDeleted =>
            $"{ActorUsername} deleted a review for {MovieLabel}",

        ActivityFeedType.ReviewLiked =>
            $"{ActorUsername} liked a review",

        ActivityFeedType.ReviewUnliked =>
            $"{ActorUsername} unliked a review",

        ActivityFeedType.UserFollowed =>
            $"{ActorUsername} followed {TargetUserLabel}",

        ActivityFeedType.UserUnfollowed =>
            $"{ActorUsername} unfollowed {TargetUserLabel}",

        ActivityFeedType.ProfileUpdated =>
            $"{ActorUsername} updated their profile",

        ActivityFeedType.AvatarUpdated =>
            $"{ActorUsername} updated their avatar",

        _ => $"{ActorUsername} did something"
    };

    public string TimeText
    {
        get
        {
            var elapsed = DateTimeOffset.Now - CreatedAt.ToLocalTime();

            if (elapsed.TotalMinutes < 1)
                return "Just now";

            if (elapsed.TotalMinutes < 60)
                return $"{(int)elapsed.TotalMinutes}m ago";

            if (elapsed.TotalHours < 24)
                return $"{(int)elapsed.TotalHours}h ago";

            if (elapsed.TotalDays < 7)
                return $"{(int)elapsed.TotalDays}d ago";

            return CreatedAt.ToLocalTime().ToString("MMM d, yyyy");
        }
    }

    public string? ReviewRatingText =>
        ReviewRating.HasValue ? ReviewRating.Value.ToString("0.0") : null;

    private string MovieLabel =>
        string.IsNullOrWhiteSpace(MovieTitle) ? "a movie" : MovieTitle;

    private string WatchlistLabel =>
        string.IsNullOrWhiteSpace(WatchlistName) ? "a watchlist" : WatchlistName;

    private string TargetUserLabel =>
        string.IsNullOrWhiteSpace(TargetUsername) ? "a user" : TargetUsername;
}
