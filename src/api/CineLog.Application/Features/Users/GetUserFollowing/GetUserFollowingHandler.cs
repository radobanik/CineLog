using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.GetUserFollowing;

public class GetUserFollowingHandler : IRequestHandler<GetUserFollowingQuery, PagedResponse<UserSummaryResponse>>
{
    private readonly IAppDbContext _context;

    public GetUserFollowingHandler(IAppDbContext context) => _context = context;

    public async Task<PagedResponse<UserSummaryResponse>> Handle(
        GetUserFollowingQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.UserFollows
            .Where(f => f.FollowerId == request.UserId)
            .Join(_context.Users,
                f => f.FollowedId,
                u => u.Id,
                (f, u) => new UserSummaryResponse(u.Id, u.UserName!, u.AvatarUrl));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<UserSummaryResponse>.Create(items, request.Page, request.PageSize, totalCount);
    }
}
