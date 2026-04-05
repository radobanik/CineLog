using MediatR;

namespace CineLog.Application.Features.Users.Watchlist.RemoveFromWatchlist;

public record RemoveFromWatchlistCommand(Guid WatchlistId, Guid MovieId) : IRequest;
