using CineLog.Application.Common;
using CineLog.Application.Features.Movies;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.Watchlist.GetWatchlist;

public class GetWatchlistHandler : IRequestHandler<GetWatchlistQuery, WatchlistDetailResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWatchlistHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<WatchlistDetailResponse> Handle(
        GetWatchlistQuery request,
        CancellationToken cancellationToken)
    {
        var watchlist = await _context.Watchlists
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.WatchlistId && w.UserId == _currentUser.UserId,
                cancellationToken)
            ?? throw new NotFoundException($"Watchlist {request.WatchlistId} not found.");

        var movies = await _context.WatchlistItems
            .AsNoTracking()
            .Where(i => i.WatchlistId == request.WatchlistId)
            .OrderByDescending(i => i.AddedAt)
            .Join(_context.Movies, i => i.MovieId, m => m.Id, (i, m) => new MovieListItemResponse(
                m.Id, m.Title, m.PosterPath, m.AverageRating, m.Type, i.AddedAt))
            .ToListAsync(cancellationToken);

        return new WatchlistDetailResponse(watchlist.Id, watchlist.Name, watchlist.CreatedAt, movies);
    }
}
