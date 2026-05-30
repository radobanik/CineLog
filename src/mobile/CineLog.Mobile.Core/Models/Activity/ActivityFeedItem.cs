namespace CineLog.Mobile.Core.Models.Activity;

public sealed class ActivityFeedItem
{
    public Guid Id { get; init; }
    public ActivityFeedType Type { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public Guid ActorId { get; init; }
    public string ActorUsername { get; init; } = string.Empty;
    public string? ActorAvatarUrl { get; init; }
    public bool IsCurrentUser { get; init; }

    public Guid? TargetUserId { get; init; }
    public string? TargetUsername { get; init; }
    public string? TargetAvatarUrl { get; init; }
    public bool IsTargetCurrentUser { get; init; }

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
    public bool HasReviewPreview => HasReviewText || ReviewRating.HasValue;
    public bool HasReviewText => !string.IsNullOrWhiteSpace(ReviewText);
    public bool HasPrimaryObject => !string.IsNullOrWhiteSpace(PrimaryObjectText);

    public string ActorDisplayName => IsCurrentUser ? "You" : ActorUsername;

    public string ActionVerb => Type switch
    {
        ActivityFeedType.MovieWatched => IsCurrentUser ? "watched" : "watched",
        ActivityFeedType.MovieWatchLaterAdded => "added",
        ActivityFeedType.MovieFavorited => "added",
        ActivityFeedType.MovieFavoriteRemoved => "removed",
        ActivityFeedType.MovieAddedToCustomWatchlist => "added",
        ActivityFeedType.ReviewCreated => "reviewed",
        ActivityFeedType.ReviewUpdated => "updated a review for",
        ActivityFeedType.ReviewDeleted => "deleted a review for",
        ActivityFeedType.ReviewLiked => "liked a review for",
        ActivityFeedType.ReviewUnliked => "unliked a review for",
        ActivityFeedType.UserFollowed => "followed",
        ActivityFeedType.UserUnfollowed => "unfollowed",
        ActivityFeedType.ProfileUpdated => IsCurrentUser ? "updated your profile" : "updated their profile",
        ActivityFeedType.AvatarUpdated => IsCurrentUser ? "updated your avatar" : "updated their avatar",
        ActivityFeedType.WatchlistCreated => "created a watchlist:",
        _ => "did something"
    };

    public string PrimaryObjectText => Type switch
    {
        ActivityFeedType.MovieWatched => MovieLabel,
        ActivityFeedType.MovieWatchLaterAdded => MovieLabel,
        ActivityFeedType.MovieFavorited => MovieLabel,
        ActivityFeedType.MovieFavoriteRemoved => MovieLabel,
        ActivityFeedType.MovieAddedToCustomWatchlist => MovieLabel,
        ActivityFeedType.ReviewCreated => MovieLabel,
        ActivityFeedType.ReviewUpdated => MovieLabel,
        ActivityFeedType.ReviewDeleted => MovieLabel,
        ActivityFeedType.ReviewLiked => MovieLabel,
        ActivityFeedType.ReviewUnliked => MovieLabel,
        ActivityFeedType.UserFollowed => TargetUserLabel,
        ActivityFeedType.UserUnfollowed => TargetUserLabel,
        ActivityFeedType.WatchlistCreated => WatchlistLabel,
        _ => string.Empty
    };

    public string SecondaryText => Type switch
    {
        ActivityFeedType.MovieWatchLaterAdded => " to Watch later",
        ActivityFeedType.MovieFavorited => " to Favorites",
        ActivityFeedType.MovieFavoriteRemoved => " from Favorites",
        ActivityFeedType.MovieAddedToCustomWatchlist => $" to {WatchlistLabel}",
        _ => string.Empty
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
        IsTargetCurrentUser
            ? "you"
            : string.IsNullOrWhiteSpace(TargetUsername)
                ? "a user"
                : TargetUsername;
}
