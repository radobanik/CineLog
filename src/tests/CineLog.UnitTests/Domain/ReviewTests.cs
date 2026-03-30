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
}
