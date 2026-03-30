using CineLog.Domain.Enums;
using CineLog.Domain.Events;
using CineLog.Domain.ValueObjects;
using MediatR;

namespace CineLog.Domain.Entities;

public class Review
{
    private readonly List<ReviewReaction> _reactions = [];
    private readonly List<INotification> _domainEvents = [];

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid MovieId { get; private set; }
    public Rating Rating { get; private set; } = null!;
    public string? ReviewText { get; private set; }
    public bool ContainsSpoilers { get; private set; }
    public DateOnly? WatchedOn { get; private set; }
    public bool IsLiked { get; private set; }
    public int LikesCount { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<ReviewReaction> Reactions => _reactions.AsReadOnly();
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    private Review() { }

    public static Review Create(
        Guid userId,
        Guid movieId,
        Rating rating,
        string? reviewText,
        bool containsSpoilers)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MovieId = movieId,
            Rating = rating,
            ReviewText = reviewText,
            ContainsSpoilers = containsSpoilers,
            CreatedAt = DateTimeOffset.UtcNow
        };

        review._domainEvents.Add(new ReviewCreatedEvent(review.Id, userId, movieId));

        return review;
    }

    public void AddReaction(Guid userId, ReactionType type)
    {
        var reaction = ReviewReaction.Create(Id, userId, type);
        _reactions.Add(reaction);
        _domainEvents.Add(new ReviewReactedEvent(Id, userId, type));
    }

    public void Update(Rating rating, string? reviewText, bool containsSpoilers)
    {
        Rating = rating;
        ReviewText = reviewText;
        ContainsSpoilers = containsSpoilers;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
