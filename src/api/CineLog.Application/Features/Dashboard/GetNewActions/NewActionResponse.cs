namespace CineLog.Application.Features.Dashboard.GetNewActions;

public record NewActionResponse(
    ReviewInfo Review,
    MovieInfo Movie,
    UserInfo User);

public record ReviewInfo(
    Guid Id,
    decimal Rating,
    string? ReviewText,
    bool ContainsSpoilers,
    DateOnly? WatchedOn,
    int LikesCount,
    DateTimeOffset CreatedAt);

public record MovieInfo(
    Guid Id,
    string Title,
    string? PosterPath);

public record UserInfo(
    Guid Id,
    string Username,
    string? AvatarUrl);
