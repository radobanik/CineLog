using CineLog.Domain.Enums;
using CineLog.Domain.ValueObjects;

namespace CineLog.Domain.Entities;

public class User
{
    private readonly List<Review> _reviews = [];
    private readonly List<UserFollow> _following = [];
    private readonly List<UserFollow> _followers = [];

    public Guid Id { get; private set; }
    public Username Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public UserRole Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<UserFollow> Following => _following.AsReadOnly();
    public IReadOnlyCollection<UserFollow> Followers => _followers.AsReadOnly();

    private User() { }

    public static User Create(string username, string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = Username.Create(username),
            Email = email,
            PasswordHash = passwordHash,
            Role = UserRole.User,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void AssignRole(UserRole role) => Role = role;
}
