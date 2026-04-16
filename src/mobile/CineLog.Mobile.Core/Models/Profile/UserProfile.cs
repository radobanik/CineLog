namespace CineLog.Mobile.Core.Models.Profile;

public sealed class UserProfile
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;
    public string AvatarUrl { get; init; } = string.Empty;
    public int FilmsCount { get; init; }
    public int FollowersCount { get; init; }
    public int FollowingCount { get; init; }
}
