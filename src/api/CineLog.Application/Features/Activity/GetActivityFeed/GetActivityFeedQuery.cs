using MediatR;

namespace CineLog.Application.Features.Activity.GetActivityFeed;

public record GetActivityFeedQuery(int Count = 50) : IRequest<List<ActivityFeedItemResponse>>;
