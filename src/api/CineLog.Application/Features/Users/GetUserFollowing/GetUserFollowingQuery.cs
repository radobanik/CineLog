using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.Users.GetUserFollowing;

public record GetUserFollowingQuery(Guid UserId, int Page = 1, int PageSize = 20)
    : IRequest<PagedResponse<UserSummaryResponse>>;
