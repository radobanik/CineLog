namespace CineLog.Domain.Enums;

public enum ActivityType
{
    MovieWatched = 1,
    MovieWatchLaterAdded = 2,
    MovieFavorited = 3,
    MovieAddedToCustomWatchlist = 4,
    ReviewCreated = 5,
    ReviewLiked = 6,
    UserFollowed = 7,

    MovieFavoriteRemoved = 8,
    ReviewUnliked = 9,
    UserUnfollowed = 10,
    ProfileUpdated = 11,
    AvatarUpdated = 12,
    ReviewUpdated = 13,
    ReviewDeleted = 14
}
