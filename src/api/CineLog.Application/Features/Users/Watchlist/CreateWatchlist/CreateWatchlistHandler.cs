using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using MediatR;
using WatchlistEntity = CineLog.Domain.Entities.Watchlist;

namespace CineLog.Application.Features.Users.Watchlist.CreateWatchlist;

public class CreateWatchlistHandler : IRequestHandler<CreateWatchlistCommand, Guid>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateWatchlistHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Guid> Handle(CreateWatchlistCommand request, CancellationToken cancellationToken)
    {
        var watchlist = WatchlistEntity.Create(_currentUser.UserId, request.Name);
        await _context.Watchlists.AddAsync(watchlist, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return watchlist.Id;
    }
}
