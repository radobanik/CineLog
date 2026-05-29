using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class ActivityLogSeeder
{
    private static readonly ActivityType[] ActivityTypes =
        Enum.GetValues<ActivityType>()
            .OrderBy(type => (int)type)
            .ToArray();

    internal static async Task SeedAsync(IAppDbContext context)
    {
        var users = await context.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var movies = await context.Movies
            .OrderBy(m => m.Title)
            .ToListAsync();

        var reviews = await context.Reviews
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        var watchlists = await context.Watchlists
            .ToListAsync();

        if (users.Count == 0 || movies.Count == 0 || reviews.Count == 0)
            return;

        var baseCreatedAt = DateTimeOffset.UtcNow.AddDays(-7);

        for (var userIndex = 0; userIndex < users.Count; userIndex++)
        {
            var actor = users[userIndex];
            var targetUser = users[(userIndex + 1) % users.Count];

            var watchedWatchlist = EnsureWatchlist(
                context,
                watchlists,
                actor.Id,
                WatchlistType.Watched,
                "Watched");

            var watchLaterWatchlist = EnsureWatchlist(
                context,
                watchlists,
                actor.Id,
                WatchlistType.WatchLater,
                "Watch later");

            var customWatchlist = EnsureWatchlist(
                context,
                watchlists,
                actor.Id,
                WatchlistType.Custom,
                "Activity picks");

            for (var typeIndex = 0; typeIndex < ActivityTypes.Length; typeIndex++)
            {
                var type = ActivityTypes[typeIndex];
                var movie = movies[(userIndex + typeIndex) % movies.Count];
                var ownReview = FindReviewForUser(reviews, actor.Id, userIndex + typeIndex);
                var targetReview = FindReviewForUser(reviews, targetUser.Id, userIndex + typeIndex + 3);
                var createdAt = baseCreatedAt.AddMinutes((userIndex * ActivityTypes.Length) + typeIndex);

                await SeedActivityAsync(
                    context,
                    actor.Id,
                    targetUser.Id,
                    movie.Id,
                    ownReview,
                    targetReview,
                    watchedWatchlist,
                    watchLaterWatchlist,
                    customWatchlist,
                    type,
                    createdAt);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedActivityAsync(
        IAppDbContext context,
        Guid actorUserId,
        Guid targetUserId,
        Guid movieId,
        Review ownReview,
        Review targetReview,
        Watchlist watchedWatchlist,
        Watchlist watchLaterWatchlist,
        Watchlist customWatchlist,
        ActivityType type,
        DateTimeOffset createdAt)
    {
        switch (type)
        {
            case ActivityType.MovieWatched:
                await AddWatchlistItemIfMissingAsync(context, watchedWatchlist.Id, movieId);
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    movieId: movieId,
                    watchlistId: watchedWatchlist.Id,
                    createdAt: createdAt);
                break;

            case ActivityType.MovieWatchLaterAdded:
                await AddWatchlistItemIfMissingAsync(context, watchLaterWatchlist.Id, movieId);
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    movieId: movieId,
                    watchlistId: watchLaterWatchlist.Id,
                    createdAt: createdAt);
                break;

            case ActivityType.MovieFavorited:
            case ActivityType.MovieFavoriteRemoved:
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    movieId: movieId,
                    createdAt: createdAt);
                break;

            case ActivityType.MovieAddedToCustomWatchlist:
                await AddWatchlistItemIfMissingAsync(context, customWatchlist.Id, movieId);
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    movieId: movieId,
                    watchlistId: customWatchlist.Id,
                    createdAt: createdAt);
                break;

            case ActivityType.WatchlistCreated:
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    watchlistId: customWatchlist.Id,
                    createdAt: createdAt);
                break;

            case ActivityType.ReviewCreated:
            case ActivityType.ReviewUpdated:
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    movieId: ownReview.MovieId,
                    reviewId: ownReview.Id,
                    createdAt: createdAt);
                break;

            case ActivityType.ReviewDeleted:
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    movieId: ownReview.MovieId,
                    createdAt: createdAt);
                break;

            case ActivityType.ReviewLiked:
            case ActivityType.ReviewUnliked:
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    targetUserId: targetReview.UserId,
                    movieId: targetReview.MovieId,
                    reviewId: targetReview.Id,
                    createdAt: createdAt);
                break;

            case ActivityType.UserFollowed:
            case ActivityType.UserUnfollowed:
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    targetUserId: targetUserId,
                    createdAt: createdAt);
                break;

            case ActivityType.ProfileUpdated:
            case ActivityType.AvatarUpdated:
                await AddActivityIfMissingAsync(
                    context,
                    actorUserId,
                    type,
                    createdAt: createdAt);
                break;
        }
    }

    private static Watchlist EnsureWatchlist(
        IAppDbContext context,
        List<Watchlist> watchlists,
        Guid userId,
        WatchlistType type,
        string name)
    {
        var watchlist = type == WatchlistType.Custom
            ? watchlists.FirstOrDefault(w => w.UserId == userId && w.Type == type && w.Name == name)
            : watchlists.FirstOrDefault(w => w.UserId == userId && w.Type == type);

        if (watchlist is not null)
            return watchlist;

        watchlist = type == WatchlistType.Custom
            ? Watchlist.CreateCustom(userId, name)
            : Watchlist.CreateDefault(userId, name, type);

        context.Watchlists.Add(watchlist);
        watchlists.Add(watchlist);

        return watchlist;
    }

    private static Review FindReviewForUser(
        IReadOnlyList<Review> reviews,
        Guid userId,
        int index)
    {
        var userReviews = reviews
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.CreatedAt)
            .ToList();

        if (userReviews.Count > 0)
            return userReviews[index % userReviews.Count];

        return reviews[index % reviews.Count];
    }

    private static async Task AddWatchlistItemIfMissingAsync(
        IAppDbContext context,
        Guid watchlistId,
        Guid movieId)
    {
        var exists = await context.WatchlistItems.AnyAsync(i =>
            i.WatchlistId == watchlistId &&
            i.MovieId == movieId);

        if (!exists)
            context.WatchlistItems.Add(WatchlistItem.Create(watchlistId, movieId));
    }

    private static async Task AddActivityIfMissingAsync(
        IAppDbContext context,
        Guid actorUserId,
        ActivityType type,
        Guid? targetUserId = null,
        Guid? movieId = null,
        Guid? reviewId = null,
        Guid? watchlistId = null,
        DateTimeOffset? createdAt = null)
    {
        var exists = await context.ActivityLogs.AnyAsync(a =>
            a.ActorUserId == actorUserId &&
            a.Type == type &&
            a.TargetUserId == targetUserId &&
            a.MovieId == movieId &&
            a.ReviewId == reviewId &&
            a.WatchlistId == watchlistId);

        if (exists)
            return;

        context.ActivityLogs.Add(ActivityLog.Create(
            actorUserId,
            type,
            targetUserId,
            movieId,
            reviewId,
            watchlistId,
            createdAt));
    }
}
