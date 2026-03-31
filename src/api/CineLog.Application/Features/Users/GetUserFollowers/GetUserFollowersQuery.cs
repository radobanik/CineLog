using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.Users.GetUserFollowers;

public record GetUserFollowersQuery(Guid UserId, int Page = 1, int PageSize = 20)
    : IRequest<PagedResponse<UserSummaryResponse>>;
