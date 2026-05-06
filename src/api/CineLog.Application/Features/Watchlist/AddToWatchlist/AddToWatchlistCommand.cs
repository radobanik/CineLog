using MediatR;

namespace CineLog.Application.Features.Watchlist.AddToWatchlist;

public record AddToWatchlistCommand(Guid WatchlistId, Guid MovieId) : IRequest;
