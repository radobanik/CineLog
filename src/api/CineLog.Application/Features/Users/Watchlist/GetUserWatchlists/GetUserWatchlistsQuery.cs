using MediatR;

namespace CineLog.Application.Features.Users.Watchlist.GetUserWatchlists;

public record GetUserWatchlistsQuery : IRequest<List<WatchlistSummaryResponse>>;

public record WatchlistSummaryResponse(Guid Id, string Name, int ItemCount, DateTimeOffset CreatedAt);
