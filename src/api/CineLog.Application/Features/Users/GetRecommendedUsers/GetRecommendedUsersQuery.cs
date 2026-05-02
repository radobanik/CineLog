using MediatR;

namespace CineLog.Application.Features.Users.GetRecommendedUsers;

public record GetRecommendedUsersQuery(int Limit = 10) : IRequest<List<DiscoverUserResponse>>;

