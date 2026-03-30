namespace CineLog.Application.Features.Users;

public record UserProfileResponse(
    Guid Id,
    string Username,
    string? Bio,
    string? AvatarUrl,
    int FilmsCount,
    int FollowersCount,
    int FollowingCount);
