using MediatR;

namespace CineLog.Application.Features.Users.Watchlist.DeleteWatchlist;

public record DeleteWatchlistCommand(Guid WatchlistId) : IRequest;
