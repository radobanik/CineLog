using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.SearchUsers;

public sealed class SearchUsersHandler(
    IAppDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<SearchUsersQuery, PagedResponse<DiscoverUserResponse>>
{
    public async Task<PagedResponse<DiscoverUserResponse>> Handle(
        SearchUsersQuery request,
        CancellationToken ct)
    {
        var currentUserId = currentUser.UserId;
        var normalized = request.Query.Trim().ToLowerInvariant();

        var baseQuery = db.Users
            .Where(u =>
                u.Id != currentUserId &&
                u.UserName != null &&
                u.UserName.ToLower().Contains(normalized));

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderBy(u => u.UserName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .Select(u => new DiscoverUserResponse(
                u.Id,
                u.Username,
                u.AvatarUrl,
                u.ReviewCount,
                u.IsFollowing))
            .ToListAsync(ct);

        return PagedResponse<DiscoverUserResponse>.Create(
            items,
            request.Page,
            request.PageSize,
            totalCount);
    }
}
