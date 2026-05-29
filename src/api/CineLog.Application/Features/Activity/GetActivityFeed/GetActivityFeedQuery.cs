using MediatR;

namespace CineLog.Application.Features.Activity.GetActivityFeed;

public record GetActivityFeedQuery(int Skip = 0, int Count = 25) : IRequest<List<ActivityFeedItemResponse>>;
