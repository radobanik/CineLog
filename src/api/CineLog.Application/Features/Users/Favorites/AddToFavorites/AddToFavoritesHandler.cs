using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.Favorites.AddToFavorites;

public class AddToFavoritesHandler : IRequestHandler<AddToFavoritesCommand>
{
    private readonly IAppDbContext _context;
    private readonly IMovieRepository _movieRepository;
    private readonly ICurrentUserService _currentUser;

    public AddToFavoritesHandler(
        IAppDbContext context,
        IMovieRepository movieRepository,
        ICurrentUserService currentUser)
    {
        _context = context;
        _movieRepository = movieRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(AddToFavoritesCommand request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        var exists = await _context.UserFavorites
            .AnyAsync(f => f.UserId == _currentUser.UserId && f.MovieId == request.MovieId, cancellationToken);

        if (exists)
            throw new ConflictException("Movie is already in your favorites.");

        await _context.UserFavorites.AddAsync(
            UserFavorite.Create(_currentUser.UserId, request.MovieId), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
