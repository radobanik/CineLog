using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Reviews.DeleteReview;

public class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAppDbContext _context;

    public DeleteReviewHandler(
        IReviewRepository reviewRepository,
        ICurrentUserService currentUser,
        IAppDbContext context)
    {
        _reviewRepository = reviewRepository;
        _currentUser = currentUser;
        _context = context;
    }

    public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new NotFoundException($"Review {request.ReviewId} not found.");

        if (review.UserId != _currentUser.UserId && !_currentUser.IsAdmin)
            throw new UnauthorizedAccessException("You are not the author of this review.");

        await _context.ActivityLogs.AddAsync(
            ActivityLog.Create(
                _currentUser.UserId,
                ActivityType.ReviewDeleted,
                movieId: review.MovieId,
                reviewId: review.Id),
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await _reviewRepository.DeleteAsync(review, cancellationToken);
    }
}
