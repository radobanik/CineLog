using CineLog.Application.Common;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Activity.GetActivityFeed;

public class GetActivityFeedHandler : IRequestHandler<GetActivityFeedQuery, List<ActivityFeedItemResponse>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetActivityFeedHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<ActivityFeedItemResponse>> Handle(
        GetActivityFeedQuery request,
        CancellationToken cancellationToken)
    {
        var count = Math.Clamp(request.Count, 1, 100);

        var followedUserIds = await _db.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowerId == _currentUser.UserId)
            .Select(f => f.FollowedId)
            .ToListAsync(cancellationToken);

        var visibleActorIds = followedUserIds
            .Append(_currentUser.UserId)
            .ToHashSet();

        var activities = await _db.ActivityLogs
            .AsNoTracking()
            .Where(a =>
                visibleActorIds.Contains(a.ActorUserId) &&
                (
                    a.ActorUserId == _currentUser.UserId ||
                    a.Type != ActivityType.MovieAddedToCustomWatchlist
                ))
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

        var userIds = activities
            .SelectMany(a => new[] { a.ActorUserId, a.TargetUserId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var movieIds = activities
            .Where(a => a.MovieId.HasValue)
            .Select(a => a.MovieId!.Value)
            .Distinct()
            .ToList();

        var reviewIds = activities
            .Where(a => a.ReviewId.HasValue)
            .Select(a => a.ReviewId!.Value)
            .Distinct()
            .ToList();

        var watchlistIds = activities
            .Where(a => a.WatchlistId.HasValue)
            .Select(a => a.WatchlistId!.Value)
            .Distinct()
            .ToList();

        var users = await _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new ActivityUserInfo(u.Id, u.UserName!, u.AvatarUrl))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var movies = await _db.Movies
            .AsNoTracking()
            .Where(m => movieIds.Contains(m.Id))
            .Select(m => new ActivityMovieInfo(m.Id, m.Title, m.PosterPath))
            .ToDictionaryAsync(m => m.Id, cancellationToken);

        var reviews = await _db.Reviews
            .AsNoTracking()
            .Where(r => reviewIds.Contains(r.Id))
            .Select(r => new ActivityReviewInfo(
                r.Id,
                r.Rating.Value,
                r.ReviewText,
                r.ContainsSpoilers,
                r.CreatedAt))
            .ToDictionaryAsync(r => r.Id, cancellationToken);

        var watchlists = await _db.Watchlists
            .AsNoTracking()
            .Where(w => watchlistIds.Contains(w.Id))
            .Select(w => new ActivityWatchlistInfo(w.Id, w.Name))
            .ToDictionaryAsync(w => w.Id, cancellationToken);

        return activities
            .Where(a => users.ContainsKey(a.ActorUserId))
            .Select(a => new ActivityFeedItemResponse(
                a.Id,
                a.Type,
                a.CreatedAt,
                users[a.ActorUserId],
                a.TargetUserId.HasValue && users.TryGetValue(a.TargetUserId.Value, out var targetUser)
                    ? targetUser
                    : null,
                a.MovieId.HasValue && movies.TryGetValue(a.MovieId.Value, out var movie)
                    ? movie
                    : null,
                a.ReviewId.HasValue && reviews.TryGetValue(a.ReviewId.Value, out var review)
                    ? review
                    : null,
                a.WatchlistId.HasValue && watchlists.TryGetValue(a.WatchlistId.Value, out var watchlist)
                    ? watchlist
                    : null))
            .ToList();
    }
}
