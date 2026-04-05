using MediatR;

namespace CineLog.Application.Features.Users.Favorites.AddToFavorites;

public record AddToFavoritesCommand(Guid MovieId) : IRequest;
