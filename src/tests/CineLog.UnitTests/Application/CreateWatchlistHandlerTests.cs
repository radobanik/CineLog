using CineLog.Application.Common;
using CineLog.Application.Features.Watchlist.CreateWatchlist;
using CineLog.Domain.Enums;
using CineLog.UnitTests.TestHelpers;
using FluentAssertions;
using NSubstitute;

namespace CineLog.UnitTests.Application;

public class CreateWatchlistHandlerTests
{
    [Fact]
    public async Task Handle_CreatesWatchlistAndPrivateActivityLog()
    {
        var currentUserId = Guid.NewGuid();
        await using var ctx = TestAppDbContext.Create();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(currentUserId);

        var handler = new CreateWatchlistHandler(ctx, currentUser);

        var watchlistId = await handler.Handle(
            new CreateWatchlistCommand("Weekend picks"),
            CancellationToken.None);

        var watchlist = ctx.Watchlists.Single();
        watchlist.Id.Should().Be(watchlistId);
        watchlist.UserId.Should().Be(currentUserId);
        watchlist.Name.Should().Be("Weekend picks");
        watchlist.Type.Should().Be(WatchlistType.Custom);

        var activity = ctx.ActivityLogs.Single();
        activity.ActorUserId.Should().Be(currentUserId);
        activity.Type.Should().Be(ActivityType.WatchlistCreated);
        activity.WatchlistId.Should().Be(watchlistId);
        activity.TargetUserId.Should().BeNull();
        activity.MovieId.Should().BeNull();
        activity.ReviewId.Should().BeNull();
    }
}
