using CineLog.Domain.Enums;

namespace CineLog.Domain.Entities;

public class ActivityLog
{
    public Guid Id { get; private set; }
    public Guid ActorUserId { get; private set; }
    public Guid? TargetUserId { get; private set; }
    public Guid? MovieId { get; private set; }
    public Guid? ReviewId { get; private set; }
    public Guid? WatchlistId { get; private set; }
    public ActivityType Type { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ActivityLog() { }

    public static ActivityLog Create(
        Guid actorUserId,
        ActivityType type,
        Guid? targetUserId = null,
        Guid? movieId = null,
        Guid? reviewId = null,
        Guid? watchlistId = null,
        DateTimeOffset? createdAt = null)
    {
        return new ActivityLog
        {
            Id = Guid.NewGuid(),
            ActorUserId = actorUserId,
            TargetUserId = targetUserId,
            MovieId = movieId,
            ReviewId = reviewId,
            WatchlistId = watchlistId,
            Type = type,
            CreatedAt = createdAt ?? DateTimeOffset.UtcNow
        };
    }
}
