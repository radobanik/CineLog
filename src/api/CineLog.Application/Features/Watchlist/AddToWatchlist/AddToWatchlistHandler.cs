using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Watchlist.AddToWatchlist;

public class AddToWatchlistHandler : IRequestHandler<AddToWatchlistCommand>
{
    private readonly IAppDbContext _context;
    private readonly IMovieRepository _movieRepository;
    private readonly ICurrentUserService _currentUser;

    public AddToWatchlistHandler(
        IAppDbContext context,
        IMovieRepository movieRepository,
        ICurrentUserService currentUser)
    {
        _context = context;
        _movieRepository = movieRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(AddToWatchlistCommand request, CancellationToken cancellationToken)
    {
        var watchlist = await _context.Watchlists
            .FirstOrDefaultAsync(w => w.Id == request.WatchlistId && w.UserId == _currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"Watchlist {request.WatchlistId} not found.");

        var movieExists = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        var alreadyAdded = await _context.WatchlistItems
            .AnyAsync(i => i.WatchlistId == request.WatchlistId && i.MovieId == request.MovieId, cancellationToken);

        if (alreadyAdded)
            throw new ConflictException("Movie is already in this watchlist.");

        await _context.WatchlistItems.AddAsync(
            WatchlistItem.Create(request.WatchlistId, request.MovieId), cancellationToken);

        var activityType = watchlist.Type switch
        {
            WatchlistType.Watched => ActivityType.MovieWatched,
            WatchlistType.WatchLater => ActivityType.MovieWatchLaterAdded,
            _ => ActivityType.MovieAddedToCustomWatchlist
        };

        await _context.ActivityLogs.AddAsync(
            ActivityLog.Create(
                _currentUser.UserId,
                activityType,
                movieId: request.MovieId,
                watchlistId: request.WatchlistId),
            cancellationToken);

        if (watchlist.Type == WatchlistType.Watched)
        {
            var watchLaterId = await _context.Watchlists
                .Where(w => w.UserId == _currentUser.UserId && w.Type == WatchlistType.WatchLater)
                .Select(w => (Guid?)w.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (watchLaterId.HasValue)
            {
                var watchLaterItem = await _context.WatchlistItems
                    .FirstOrDefaultAsync(i =>
                        i.WatchlistId == watchLaterId.Value &&
                        i.MovieId == request.MovieId,
                        cancellationToken);

                if (watchLaterItem is not null)
                    _context.WatchlistItems.Remove(watchLaterItem);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
