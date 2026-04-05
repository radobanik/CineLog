using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Events;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class ReviewTests
{
    [Fact]
    public void Create_RaisesReviewCreatedEvent()
    {
        var userId = Guid.NewGuid();
        var movieId = Guid.NewGuid();

        var review = Review.Create(userId, movieId, 4.0m, "Great film", false);

        review.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ReviewCreatedEvent>()
            .Which.Should().BeEquivalentTo(new
            {
                ReviewId = review.Id,
                UserId = userId,
                MovieId = movieId
            });
    }

    [Fact]
    public void AddReaction_RaisesReviewReactedEvent()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 3.0m, null, false);
        review.ClearDomainEvents();

        var reactingUserId = Guid.NewGuid();
        review.AddReaction(reactingUserId, ReactionType.Like);

        review.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ReviewReactedEvent>()
            .Which.Should().BeEquivalentTo(new
            {
                ReviewId = review.Id,
                ReactedByUserId = reactingUserId,
                ReactionType = ReactionType.Like
            });
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 2.5m, null, false);
        review.DomainEvents.Should().NotBeEmpty();

        review.ClearDomainEvents();

        review.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Update_ChangesCoreFields()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 2.0m, "Meh", false);
        review.ClearDomainEvents();

        review.Update(5.0m, "Actually amazing", true);

        review.Rating.Value.Should().Be(5.0m);
        review.ReviewText.Should().Be("Actually amazing");
        review.ContainsSpoilers.Should().BeTrue();
    }

    [Fact]
    public void Update_SetsUpdatedAt()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 3.0m, null, false);
        review.UpdatedAt.Should().BeNull();

        var before = DateTimeOffset.UtcNow;
        review.Update(4.0m, "Better on rewatch", false);

        review.UpdatedAt.Should().NotBeNull();
        review.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void RemoveReaction_ExistingReaction_RemovesIt()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 3.0m, null, false);
        var userId = Guid.NewGuid();
        review.AddReaction(userId, ReactionType.Like);
        review.Reactions.Should().HaveCount(1);

        review.RemoveReaction(userId, ReactionType.Like);

        review.Reactions.Should().BeEmpty();
    }

    [Fact]
    public void RemoveReaction_NonExistentReaction_DoesNothing()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 3.0m, null, false);

        var act = () => review.RemoveReaction(Guid.NewGuid(), ReactionType.Like);

        act.Should().NotThrow();
        review.Reactions.Should().BeEmpty();
    }

    [Fact]
    public void AddReaction_MultipleUsers_EachHasOwnReaction()
    {
        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 3.0m, null, false);

        review.AddReaction(Guid.NewGuid(), ReactionType.Like);
        review.AddReaction(Guid.NewGuid(), ReactionType.Like);

        review.Reactions.Should().HaveCount(2);
    }

    [Fact]
    public void Create_SetsCreatedAt_ToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var review = Review.Create(Guid.NewGuid(), Guid.NewGuid(), 4.0m, null, false);

        review.CreatedAt.Should().BeOnOrAfter(before);
        review.CreatedAt.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
        review.UpdatedAt.Should().BeNull();
    }
}
