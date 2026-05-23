using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Reviews.ToggleLike;

public class ToggleLikeHandler : IRequestHandler<ToggleLikeCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPublisher _publisher;
    private readonly IAppDbContext _context;

    public ToggleLikeHandler(
        IReviewRepository reviewRepository,
        ICurrentUserService currentUser,
        IPublisher publisher,
        IAppDbContext context)
    {
        _reviewRepository = reviewRepository;
        _currentUser = currentUser;
        _publisher = publisher;
        _context = context;
    }

    public async Task Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new NotFoundException($"Review {request.ReviewId} not found.");

        var existingLike = review.Reactions
            .FirstOrDefault(r => r.UserId == _currentUser.UserId && r.Type == ReactionType.Like);

        var isAddingLike = existingLike is null;

        if (existingLike is not null)
        {
            review.RemoveReaction(_currentUser.UserId, ReactionType.Like);
        }
        else
        {
            review.AddReaction(_currentUser.UserId, ReactionType.Like);
        }

        await _reviewRepository.UpdateReactionsAsync(review, cancellationToken);

        if (isAddingLike)
        {
            await _context.ActivityLogs.AddAsync(
                ActivityLog.Create(
                    _currentUser.UserId,
                    ActivityType.ReviewLiked,
                    movieId: review.MovieId,
                    reviewId: review.Id),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        foreach (var domainEvent in review.DomainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        review.ClearDomainEvents();
    }
}
