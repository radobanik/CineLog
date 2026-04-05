using MediatR;

namespace CineLog.Application.Features.Users.Watchlist.AddToWatchlist;

public record AddToWatchlistCommand(Guid WatchlistId, Guid MovieId) : IRequest;
