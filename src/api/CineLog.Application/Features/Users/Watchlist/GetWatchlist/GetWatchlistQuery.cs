using CineLog.Application.Features.Movies;
using MediatR;

namespace CineLog.Application.Features.Users.Watchlist.GetWatchlist;

public record GetWatchlistQuery(Guid WatchlistId) : IRequest<WatchlistDetailResponse>;

public record WatchlistDetailResponse(Guid Id, string Name, DateTimeOffset CreatedAt, List<MovieListItemResponse> Movies);
