using MediatR;

namespace CineLog.Application.Features.Watchlist.RemoveFromWatchlist;

public record RemoveFromWatchlistCommand(Guid WatchlistId, Guid MovieId) : IRequest;
