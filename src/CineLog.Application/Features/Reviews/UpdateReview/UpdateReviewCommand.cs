using MediatR;

namespace CineLog.Application.Features.Reviews.UpdateReview;

public record UpdateReviewCommand(
    Guid ReviewId,
    decimal Rating,
    string? ReviewText,
    bool ContainsSpoilers,
    List<string> Tags) : IRequest<ReviewResponse>;
