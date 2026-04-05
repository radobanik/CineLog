using MediatR;

namespace CineLog.Application.Features.Users.Watchlist.CreateWatchlist;

public record CreateWatchlistCommand(string Name) : IRequest<Guid>;
