namespace CineLog.Application.Features.Reviews;

public record ReviewResponse(
    Guid Id,
    Guid UserId,
    string Username,
    string? AvatarUrl,
    string MovieTitle,
    string? MoviePosterPath,
    decimal Rating,
    string? ReviewText,
    bool ContainsSpoilers,
    int LikesCount,
    bool IsLiked,
    DateTimeOffset CreatedAt);
