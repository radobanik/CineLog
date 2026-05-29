using CineLog.Application.Common;
using CineLog.Application.Features.Activity.GetActivityFeed;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.ValueObjects;
using CineLog.UnitTests.TestHelpers;
using FluentAssertions;
using NSubstitute;

namespace CineLog.UnitTests.Application;

public class GetActivityFeedHandlerTests
{
    private static (TestAppDbContext ctx, ICurrentUserService currentUser, GetActivityFeedHandler handler) BuildSut(Guid currentUserId)
    {
        var ctx = TestAppDbContext.Create();
        var currentUser = Substitute.For<ICurrentUserService>();

        currentUser.UserId.Returns(currentUserId);

        return (ctx, currentUser, new GetActivityFeedHandler(ctx, currentUser));
    }

    [Fact]
    public async Task Handle_ReturnsCurrentUserAndFollowedUserActivities()
    {
        var currentUserId = Guid.NewGuid();
        var followedUserId = Guid.NewGuid();
        var strangerUserId = Guid.NewGuid();

        var (ctx, _, handler) = BuildSut(currentUserId);

        var currentUser = CreateUser(currentUserId, "alice");
        var followedUser = CreateUser(followedUserId, "bob");
        var stranger = CreateUser(strangerUserId, "charlie");

        var movie = Movie.Create(1, "Test Movie", MovieType.Movie);
        movie.UpdateDetails(null, "/poster.jpg", null, null, null);

        var review = Review.Create(
            followedUserId,
            movie.Id,
            Rating.Create(4.5m),
            "Great review",
            false);

        ctx.Users.AddRange(currentUser, followedUser, stranger);
        ctx.Movies.Add(movie);
        ctx.Reviews.Add(review);
        ctx.UserFollows.Add(UserFollow.Create(currentUserId, followedUserId));

        var currentUserActivity = ActivityLog.Create(
            currentUserId,
            ActivityType.MovieFavorited,
            movieId: movie.Id,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-1));

        var followedUserActivity = ActivityLog.Create(
            followedUserId,
            ActivityType.ReviewCreated,
            movieId: movie.Id,
            reviewId: review.Id,
            createdAt: DateTimeOffset.UtcNow);

        var strangerActivity = ActivityLog.Create(
            strangerUserId,
            ActivityType.MovieFavorited,
            movieId: movie.Id,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(1));

        ctx.ActivityLogs.AddRange(currentUserActivity, followedUserActivity, strangerActivity);
        await ctx.SaveChangesAsync();

        var result = await handler.Handle(new GetActivityFeedQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(a => a.Id).Should().Contain(new[] { currentUserActivity.Id, followedUserActivity.Id });
        result.Select(a => a.Id).Should().NotContain(strangerActivity.Id);

        var reviewActivity = result.Single(a => a.Id == followedUserActivity.Id);
        reviewActivity.Actor.Username.Should().Be("bob");
        reviewActivity.Movie!.Title.Should().Be("Test Movie");
        reviewActivity.Movie.PosterPath.Should().Be("/poster.jpg");
        reviewActivity.Review!.Rating.Should().Be(4.5m);
        reviewActivity.Review.ReviewText.Should().Be("Great review");
    }

    [Fact]
    public async Task Handle_HidesFollowedUsersCustomWatchlistActivities()
    {
        var currentUserId = Guid.NewGuid();
        var followedUserId = Guid.NewGuid();

        var (ctx, _, handler) = BuildSut(currentUserId);

        var movie = Movie.Create(1, "Test Movie", MovieType.Movie);
        var currentUserWatchlist = Watchlist.CreateCustom(currentUserId, "Alice List");
        var followedUserWatchlist = Watchlist.CreateCustom(followedUserId, "Bob List");

        ctx.Users.AddRange(
            CreateUser(currentUserId, "alice"),
            CreateUser(followedUserId, "bob"));

        ctx.Movies.Add(movie);
        ctx.Watchlists.AddRange(currentUserWatchlist, followedUserWatchlist);
        ctx.UserFollows.Add(UserFollow.Create(currentUserId, followedUserId));

        var ownCustomWatchlistActivity = ActivityLog.Create(
            currentUserId,
            ActivityType.MovieAddedToCustomWatchlist,
            movieId: movie.Id,
            watchlistId: currentUserWatchlist.Id,
            createdAt: DateTimeOffset.UtcNow);

        var followedCustomWatchlistActivity = ActivityLog.Create(
            followedUserId,
            ActivityType.MovieAddedToCustomWatchlist,
            movieId: movie.Id,
            watchlistId: followedUserWatchlist.Id,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(1));

        ctx.ActivityLogs.AddRange(ownCustomWatchlistActivity, followedCustomWatchlistActivity);
        await ctx.SaveChangesAsync();

        var result = await handler.Handle(new GetActivityFeedQuery(), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Id.Should().Be(ownCustomWatchlistActivity.Id);
        result[0].Watchlist!.Name.Should().Be("Alice List");
    }

    [Fact]
    public async Task Handle_ReturnsFollowActivitiesOnlyWhenCurrentUserIsActorOrTarget()
    {
        var currentUserId = Guid.NewGuid();
        var followedUserId = Guid.NewGuid();
        var strangerUserId = Guid.NewGuid();
        var unrelatedUserId = Guid.NewGuid();

        var (ctx, _, handler) = BuildSut(currentUserId);

        ctx.Users.AddRange(
            CreateUser(currentUserId, "alice"),
            CreateUser(followedUserId, "bob"),
            CreateUser(strangerUserId, "charlie"),
            CreateUser(unrelatedUserId, "dave"));

        var currentUserFollowedSomeone = ActivityLog.Create(
            currentUserId,
            ActivityType.UserFollowed,
            targetUserId: followedUserId,
            createdAt: DateTimeOffset.UtcNow);

        var someoneFollowedCurrentUser = ActivityLog.Create(
            strangerUserId,
            ActivityType.UserFollowed,
            targetUserId: currentUserId,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(1));

        var unrelatedFollow = ActivityLog.Create(
            strangerUserId,
            ActivityType.UserFollowed,
            targetUserId: unrelatedUserId,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(2));

        var unrelatedUnfollow = ActivityLog.Create(
            strangerUserId,
            ActivityType.UserUnfollowed,
            targetUserId: unrelatedUserId,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(3));

        ctx.ActivityLogs.AddRange(
            currentUserFollowedSomeone,
            someoneFollowedCurrentUser,
            unrelatedFollow,
            unrelatedUnfollow);

        await ctx.SaveChangesAsync();

        var result = await handler.Handle(new GetActivityFeedQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(a => a.Id).Should().Contain(new[] { currentUserFollowedSomeone.Id, someoneFollowedCurrentUser.Id });
        result.Select(a => a.Id).Should().NotContain(new[] { unrelatedFollow.Id, unrelatedUnfollow.Id });
    }

    [Fact]
    public async Task Handle_AppliesOrderingPagingAndCountClamp()
    {
        var currentUserId = Guid.NewGuid();

        var (ctx, _, handler) = BuildSut(currentUserId);

        ctx.Users.Add(CreateUser(currentUserId, "alice"));

        var oldest = ActivityLog.Create(
            currentUserId,
            ActivityType.ProfileUpdated,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-3));

        var middle = ActivityLog.Create(
            currentUserId,
            ActivityType.AvatarUpdated,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-2));

        var newest = ActivityLog.Create(
            currentUserId,
            ActivityType.ProfileUpdated,
            createdAt: DateTimeOffset.UtcNow.AddMinutes(-1));

        ctx.ActivityLogs.AddRange(oldest, middle, newest);
        await ctx.SaveChangesAsync();

        var result = await handler.Handle(new GetActivityFeedQuery(Skip: 1, Count: 1), CancellationToken.None);

        result.Should().ContainSingle();
        result[0].Id.Should().Be(middle.Id);
    }

    private static User CreateUser(Guid id, string username)
    {
        return new User
        {
            Id = id,
            UserName = username,
            NormalizedUserName = username.ToUpperInvariant(),
            Email = $"{username}@cinelog.dev",
            NormalizedEmail = $"{username}@cinelog.dev".ToUpperInvariant()
        };
    }
}
