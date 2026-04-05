using CineLog.Domain.Entities;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class UserFavoriteTests
{
    [Fact]
    public void Create_SetsUserAndMovieIds()
    {
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var favorite = UserFavorite.Create(userId, movieId);

        favorite.UserId.Should().Be(userId);
        favorite.MovieId.Should().Be(movieId);
    }

    [Fact]
    public void Create_SetsAddedAt_ToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var favorite = UserFavorite.Create(Guid.NewGuid(), Guid.NewGuid());

        favorite.AddedAt.Should().BeOnOrAfter(before);
        favorite.AddedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_TwoFavorites_AreIndependent()
    {
        var a = UserFavorite.Create(Guid.NewGuid(), Guid.NewGuid());
        var b = UserFavorite.Create(Guid.NewGuid(), Guid.NewGuid());

        a.UserId.Should().NotBe(b.UserId);
        a.MovieId.Should().NotBe(b.MovieId);
    }
}
