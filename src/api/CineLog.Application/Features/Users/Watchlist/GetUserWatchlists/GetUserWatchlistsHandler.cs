using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.Watchlist.GetUserWatchlists;

public class GetUserWatchlistsHandler : IRequestHandler<GetUserWatchlistsQuery, List<WatchlistSummaryResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUserWatchlistsHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<WatchlistSummaryResponse>> Handle(
        GetUserWatchlistsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Watchlists
            .AsNoTracking()
            .Where(w => w.UserId == _currentUser.UserId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WatchlistSummaryResponse(
                w.Id,
                w.Name,
                w.Items.Count,
                w.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
