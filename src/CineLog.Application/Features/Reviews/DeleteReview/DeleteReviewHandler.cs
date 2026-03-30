using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Reviews.DeleteReview;

public class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserService _currentUser;

    public DeleteReviewHandler(IReviewRepository reviewRepository, ICurrentUserService currentUser)
    {
        _reviewRepository = reviewRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new NotFoundException($"Review {request.ReviewId} not found.");

        if (review.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new UnauthorizedAccessException("You are not the author of this review.");

        await _reviewRepository.DeleteAsync(review, cancellationToken);
    }
}
