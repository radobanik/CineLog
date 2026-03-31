using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.GetUserFollowers;

public class GetUserFollowersHandler : IRequestHandler<GetUserFollowersQuery, PagedResponse<UserSummaryResponse>>
{
    private readonly IAppDbContext _context;

    public GetUserFollowersHandler(IAppDbContext context) => _context = context;

    public async Task<PagedResponse<UserSummaryResponse>> Handle(
        GetUserFollowersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.UserFollows
            .Where(f => f.FollowedId == request.UserId)
            .Join(_context.Users,
                f => f.FollowerId,
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
