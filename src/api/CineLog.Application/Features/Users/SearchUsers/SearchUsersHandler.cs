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
        var normalized = request.Query.Trim().ToLowerInvariant();

        var query = db.Users
            .Where(u =>
                u.Id != currentUser.UserId &&
                u.UserName != null &&
                u.UserName.ToLower().Contains(normalized))
            .Select(u => new DiscoverUserResponse(
                u.Id,
                u.UserName ?? string.Empty,
                u.AvatarUrl,
                db.Reviews.Count(r => r.UserId == u.Id),
                db.UserFollows.Any(f =>
                    f.FollowerId == currentUser.UserId &&
                    f.FollowedId == u.Id)
                ))
            .OrderBy(u => u.Username);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return PagedResponse<DiscoverUserResponse>.Create(
            items,
            request.Page,
            request.PageSize,
            totalCount);
    }
}
