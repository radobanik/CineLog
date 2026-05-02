using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.GetRecommendedUsers;

public sealed class GetRecommendedUsersHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<GetRecommendedUsersQuery, List<DiscoverUserResponse>>
{
    public Task<List<DiscoverUserResponse>> Handle(
        GetRecommendedUsersQuery request,
        CancellationToken ct)
    {
        var currentUserId = currentUser.UserId;

        return db.Users
            .Where(u => u.Id != currentUserId)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName ?? string.Empty,
                u.AvatarUrl,
                ReviewCount = db.Reviews.Count(r => r.UserId == u.Id),
                IsFollowing = db.UserFollows.Any(f =>
                    f.FollowerId == currentUserId &&
                    f.FollowedId == u.Id)
            })
            .OrderByDescending(u => u.ReviewCount)
            .ThenBy(u => u.Username)
            .Take(request.Limit)
            .Select(u => new DiscoverUserResponse(
                u.Id,
                u.Username,
                u.AvatarUrl,
                u.ReviewCount,
                u.IsFollowing))
            .ToListAsync(ct);
    }
}
