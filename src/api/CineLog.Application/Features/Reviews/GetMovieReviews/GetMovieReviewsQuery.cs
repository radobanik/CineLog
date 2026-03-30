using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.Reviews.GetMovieReviews;

public record GetMovieReviewsQuery(Guid MovieId, int Page = 1, int PageSize = 20)
    : IRequest<PagedResponse<ReviewResponse>>;
