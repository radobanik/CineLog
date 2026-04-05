using MediatR;

namespace CineLog.Application.Features.Users.Favorites.RemoveFromFavorites;

public record RemoveFromFavoritesCommand(Guid MovieId) : IRequest;
