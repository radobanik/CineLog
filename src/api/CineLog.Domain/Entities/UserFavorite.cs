namespace CineLog.Domain.Entities;

public class UserFavorite
{
    public Guid UserId { get; private set; }
    public Guid MovieId { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }

    private UserFavorite() { }

    public static UserFavorite Create(Guid userId, Guid movieId)
    {
        return new UserFavorite
        {
            UserId = userId,
            MovieId = movieId,
            AddedAt = DateTimeOffset.UtcNow
        };
    }
}
