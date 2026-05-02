using MediatR;

namespace CineLog.Application.Features.Watchlist.DeleteWatchlist;

public record DeleteWatchlistCommand(Guid WatchlistId) : IRequest;
