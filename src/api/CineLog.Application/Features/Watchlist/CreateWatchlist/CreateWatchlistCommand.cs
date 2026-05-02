using MediatR;

namespace CineLog.Application.Features.Watchlist.CreateWatchlist;

public record CreateWatchlistCommand(string Name) : IRequest<Guid>;
