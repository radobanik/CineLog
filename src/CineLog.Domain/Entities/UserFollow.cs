namespace CineLog.Domain.Entities;

public class UserFollow
{
    public Guid FollowerId { get; private set; }
    public Guid FollowedId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private UserFollow() { }

    public static UserFollow Create(Guid followerId, Guid followedId)
    {
        return new UserFollow
        {
            FollowerId = followerId,
            FollowedId = followedId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
