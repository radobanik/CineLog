using CineLog.Application.Features.Movies;
using MediatR;

namespace CineLog.Application.Features.Users.Favorites.GetFavorites;

public record GetFavoritesQuery : IRequest<List<MovieListItemResponse>>;
