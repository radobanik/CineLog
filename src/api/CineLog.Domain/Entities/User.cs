using Microsoft.AspNetCore.Identity;

namespace CineLog.Domain.Entities;

public class User : IdentityUser<Guid>
{
    private readonly List<Review> _reviews = [];
    private readonly List<UserFollow> _following = [];
    private readonly List<UserFollow> _followers = [];

    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
    public IReadOnlyCollection<UserFollow> Following => _following.AsReadOnly();
    public IReadOnlyCollection<UserFollow> Followers => _followers.AsReadOnly();
}
