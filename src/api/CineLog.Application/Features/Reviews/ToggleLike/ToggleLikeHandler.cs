using CineLog.Application.Common;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Reviews.ToggleLike;

public class ToggleLikeHandler : IRequestHandler<ToggleLikeCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPublisher _publisher;

    public ToggleLikeHandler(
        IReviewRepository reviewRepository,
        ICurrentUserService currentUser,
        IPublisher publisher)
    {
        _reviewRepository = reviewRepository;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new NotFoundException($"Review {request.ReviewId} not found.");

        var existingLike = review.Reactions
            .FirstOrDefault(r => r.UserId == _currentUser.UserId && r.Type == ReactionType.Like);

        if (existingLike is not null)
        {
            review.RemoveReaction(_currentUser.UserId, ReactionType.Like);
        }
        else
        {
            review.AddReaction(_currentUser.UserId, ReactionType.Like);
        }

        await _reviewRepository.UpdateReactionsAsync(review, cancellationToken);

        foreach (var domainEvent in review.DomainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        review.ClearDomainEvents();
    }
}
