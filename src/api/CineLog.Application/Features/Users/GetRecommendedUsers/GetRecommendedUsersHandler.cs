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
        return db.Users
            .Where(u => u.Id != currentUser.UserId)
            .Select(u => new DiscoverUserResponse(
                u.Id,
                u.UserName ?? string.Empty,
                u.AvatarUrl,
                db.Reviews.Count(r => r.UserId == u.Id),
                db.UserFollows.Any(f =>
                    f.FollowerId == currentUser.UserId &&
                    f.FollowedId == u.Id)
                ))
            .OrderByDescending(u => u.ReviewCount)
            .ThenBy(u => u.Username)
            .Take(request.Limit)
            .ToListAsync(ct);
    }
}
