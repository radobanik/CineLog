using CineLog.Domain.Entities;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class WatchlistTests
{
    [Fact]
    public void Create_SetsPropertiesAndGeneratesId()
    {
        var userId = Guid.NewGuid();

        var watchlist = Watchlist.Create(userId, "My Favourites");

        watchlist.Id.Should().NotBeEmpty();
        watchlist.UserId.Should().Be(userId);
        watchlist.Name.Should().Be("My Favourites");
        watchlist.Items.Should().BeEmpty();
    }

    [Fact]
    public void Create_SetsCreatedAt_ToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var watchlist = Watchlist.Create(Guid.NewGuid(), "Watch Later");

        watchlist.CreatedAt.Should().BeOnOrAfter(before);
        watchlist.CreatedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_TwoWatchlists_HaveDistinctIds()
    {
        var userId = Guid.NewGuid();

        var a = Watchlist.Create(userId, "List A");
        var b = Watchlist.Create(userId, "List B");

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
}
