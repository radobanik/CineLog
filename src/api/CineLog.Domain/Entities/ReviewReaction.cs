using CineLog.Domain.Enums;

namespace CineLog.Domain.Entities;

public class ReviewReaction
{
    public Guid Id { get; private set; }
    public Guid ReviewId { get; private set; }
    public Guid UserId { get; private set; }
    public ReactionType Type { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ReviewReaction() { }

    internal static ReviewReaction Create(Guid reviewId, Guid userId, ReactionType type)
    {
        return new ReviewReaction
        {
            Id = Guid.NewGuid(),
            ReviewId = reviewId,
            UserId = userId,
            Type = type,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
