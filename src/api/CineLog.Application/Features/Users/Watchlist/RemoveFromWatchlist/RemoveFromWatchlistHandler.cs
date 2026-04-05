using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.Watchlist.RemoveFromWatchlist;

public class RemoveFromWatchlistHandler : IRequestHandler<RemoveFromWatchlistCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveFromWatchlistHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RemoveFromWatchlistCommand request, CancellationToken cancellationToken)
    {
        var watchlistExists = await _context.Watchlists
            .AnyAsync(w => w.Id == request.WatchlistId && w.UserId == _currentUser.UserId, cancellationToken);

        if (!watchlistExists)
            throw new NotFoundException($"Watchlist {request.WatchlistId} not found.");

        var item = await _context.WatchlistItems
            .FirstOrDefaultAsync(i => i.WatchlistId == request.WatchlistId && i.MovieId == request.MovieId,
                cancellationToken)
            ?? throw new NotFoundException("Movie is not in this watchlist.");

        _context.WatchlistItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
