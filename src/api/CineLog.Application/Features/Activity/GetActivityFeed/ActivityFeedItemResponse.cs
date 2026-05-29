using CineLog.Domain.Enums;

namespace CineLog.Application.Features.Activity.GetActivityFeed;

public record ActivityFeedItemResponse(
    Guid Id,
    ActivityType Type,
    DateTimeOffset CreatedAt,
    ActivityUserInfo Actor,
    ActivityUserInfo? TargetUser,
    ActivityMovieInfo? Movie,
    ActivityReviewInfo? Review,
    ActivityWatchlistInfo? Watchlist);

public record ActivityUserInfo(
    Guid Id,
    string Username,
    string? AvatarUrl);

public record ActivityMovieInfo(
    Guid Id,
    string Title,
    string? PosterPath);

public record ActivityReviewInfo(
    Guid Id,
    decimal Rating,
    string? ReviewText,
    bool ContainsSpoilers,
    DateTimeOffset CreatedAt);

public record ActivityWatchlistInfo(
    Guid Id,
    string Name);
