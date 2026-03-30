using MediatR;

namespace CineLog.Application.Features.Reviews.DeleteReview;

public record DeleteReviewCommand(Guid ReviewId) : IRequest;
