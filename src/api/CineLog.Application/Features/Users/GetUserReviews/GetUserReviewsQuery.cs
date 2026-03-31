using CineLog.Application.Common;
using CineLog.Application.Features.Reviews;
using MediatR;

namespace CineLog.Application.Features.Users.GetUserReviews;

public record GetUserReviewsQuery(Guid UserId, int Page = 1, int PageSize = 20)
    : IRequest<PagedResponse<ReviewResponse>>;
