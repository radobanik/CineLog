using CineLog.Application.Common;
using CineLog.Application.Features.Movies;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.Favorites.GetFavorites;

public class GetFavoritesHandler : IRequestHandler<GetFavoritesQuery, List<MovieListItemResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetFavoritesHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<MovieListItemResponse>> Handle(
        GetFavoritesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.UserFavorites
            .AsNoTracking()
            .Where(f => f.UserId == _currentUser.UserId)
            .OrderByDescending(f => f.AddedAt)
            .Join(_context.Movies, f => f.MovieId, m => m.Id, (f, m) => new MovieListItemResponse(
                m.Id, m.Title, m.PosterPath, m.AverageRating, m.Type, f.AddedAt))
            .ToListAsync(cancellationToken);
    }
}
