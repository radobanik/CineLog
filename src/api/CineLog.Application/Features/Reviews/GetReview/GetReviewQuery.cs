using MediatR;

namespace CineLog.Application.Features.Reviews.GetReview;

public record GetReviewQuery(Guid ReviewId) : IRequest<ReviewResponse>;
