using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Reviews.ReactToReview;

public class ReactToReviewHandler : IRequestHandler<ReactToReviewCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPublisher _publisher;

    public ReactToReviewHandler(
        IReviewRepository reviewRepository,
        ICurrentUserService currentUser,
        IPublisher publisher)
    {
        _reviewRepository = reviewRepository;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task Handle(ReactToReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new NotFoundException($"Review {request.ReviewId} not found.");

        review.AddReaction(_currentUser.UserId, request.ReactionType);
        await _reviewRepository.UpdateAsync(review, cancellationToken);

        foreach (var domainEvent in review.DomainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        review.ClearDomainEvents();
    }
}
