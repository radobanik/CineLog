using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.Watchlist.AddToWatchlist;

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
        var watchlistExists = await _context.Watchlists
            .AnyAsync(w => w.Id == request.WatchlistId && w.UserId == _currentUser.UserId, cancellationToken);

        if (!watchlistExists)
            throw new NotFoundException($"Watchlist {request.WatchlistId} not found.");

        var movieExists = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        var alreadyAdded = await _context.WatchlistItems
            .AnyAsync(i => i.WatchlistId == request.WatchlistId && i.MovieId == request.MovieId, cancellationToken);

        if (alreadyAdded)
            throw new ConflictException("Movie is already in this watchlist.");

        await _context.WatchlistItems.AddAsync(
            WatchlistItem.Create(request.WatchlistId, request.MovieId), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
