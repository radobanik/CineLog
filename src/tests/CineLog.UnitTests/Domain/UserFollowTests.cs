using CineLog.Domain.Entities;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class UserFollowTests
{
    [Fact]
    public void Create_SetsFollowerAndFollowed()
    {
        var followerId = Guid.NewGuid();
        var followedId = Guid.NewGuid();

        var follow = UserFollow.Create(followerId, followedId);

        follow.FollowerId.Should().Be(followerId);
        follow.FollowedId.Should().Be(followedId);
    }

    [Fact]
    public void Create_SetsCreatedAt_ToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var follow = UserFollow.Create(Guid.NewGuid(), Guid.NewGuid());

        follow.CreatedAt.Should().BeOnOrAfter(before);
        follow.CreatedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_TwoFollows_AreIndependent()
    {
        var a = UserFollow.Create(Guid.NewGuid(), Guid.NewGuid());
        var b = UserFollow.Create(Guid.NewGuid(), Guid.NewGuid());

        a.FollowerId.Should().NotBe(b.FollowerId);
    }
}
