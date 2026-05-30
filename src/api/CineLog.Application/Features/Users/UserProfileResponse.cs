namespace CineLog.Application.Features.Users;

public record UserProfileResponse(
    Guid Id,
    string Username,
    string? Bio,
    string? AvatarUrl,
    int FilmsCount,
    int FilmsThisYearCount,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowing);
