using MediatR;

namespace CineLog.Application.Features.Reviews.CreateReview;

public record CreateReviewCommand(
    Guid MovieId,
    decimal Rating,
    string? ReviewText,
    bool ContainsSpoilers) : IRequest<ReviewResponse>;
