using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.Favorites.RemoveFromFavorites;

public class RemoveFromFavoritesHandler : IRequestHandler<RemoveFromFavoritesCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveFromFavoritesHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RemoveFromFavoritesCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.UserFavorites
            .FirstOrDefaultAsync(f => f.UserId == _currentUser.UserId && f.MovieId == request.MovieId,
                cancellationToken)
            ?? throw new NotFoundException("Movie is not in your favorites.");

        _context.UserFavorites.Remove(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
