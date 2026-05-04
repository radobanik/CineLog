using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Watchlist.DeleteWatchlist;

public class DeleteWatchlistHandler : IRequestHandler<DeleteWatchlistCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteWatchlistHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteWatchlistCommand request, CancellationToken cancellationToken)
    {
        var watchlist = await _context.Watchlists
            .FirstOrDefaultAsync(w => w.Id == request.WatchlistId && w.UserId == _currentUser.UserId,
                cancellationToken)
            ?? throw new NotFoundException($"Watchlist {request.WatchlistId} not found.");

        if (watchlist.IsDefault)
            throw new ConflictException("Default watchlists cannot be deleted.");

        _context.Watchlists.Remove(watchlist);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
