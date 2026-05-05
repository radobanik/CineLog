using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class WatchlistTests
{
    [Fact]
    public void CreateCustom_SetsPropertiesAndGeneratesId()
    {
        var userId = Guid.NewGuid();

        var watchlist = Watchlist.CreateCustom(userId, "My Favourites");

        watchlist.Id.Should().NotBeEmpty();
        watchlist.UserId.Should().Be(userId);
        watchlist.Name.Should().Be("My Favourites");
        watchlist.Items.Should().BeEmpty();
    }

    [Fact]
    public void CreateCustom_SetsCreatedAt_ToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var watchlist = Watchlist.CreateCustom(Guid.NewGuid(), "Watch Later");

        watchlist.CreatedAt.Should().BeOnOrAfter(before);
        watchlist.CreatedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void CreateCustom_TwoWatchlists_HaveDistinctIds()
    {
        var userId = Guid.NewGuid();

        var a = Watchlist.CreateCustom(userId, "List A");
        var b = Watchlist.CreateCustom(userId, "List B");

        a.Id.Should().NotBe(b.Id);
    }
}

public class WatchlistItemTests
{
    [Fact]
    public void Create_SetsWatchlistAndMovieIds()
    {
        var watchlistId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var item = WatchlistItem.Create(watchlistId, movieId);

        item.WatchlistId.Should().Be(watchlistId);
        item.MovieId.Should().Be(movieId);
    }

    [Fact]
    public void Create_SetsAddedAt_ToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var item = WatchlistItem.Create(Guid.NewGuid(), Guid.NewGuid());

        item.AddedAt.Should().BeOnOrAfter(before);
        item.AddedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }
    [Fact]
    public void CreateDefault_SetsType()
    {
        var watchlist = Watchlist.CreateDefault(
            Guid.NewGuid(),
            "Watched",
            WatchlistType.Watched);

        watchlist.Type.Should().Be(WatchlistType.Watched);
        watchlist.IsDefault.Should().BeTrue();
    }
}
