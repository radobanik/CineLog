using CineLog.Application.Common;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Reviews.GetReview;

public class GetReviewHandler : IRequestHandler<GetReviewQuery, ReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetReviewHandler(IReviewRepository reviewRepository, IAppDbContext context, ICurrentUserService currentUser)
    {
        _reviewRepository = reviewRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ReviewResponse> Handle(GetReviewQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new NotFoundException($"Review {request.ReviewId} not found.");

        var username = await _context.Users
            .Where(u => u.Id == review.UserId)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var movieTitle = await _context.Movies
            .Where(m => m.Id == review.MovieId)
            .Select(m => m.Title)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        var isLiked = await _context.ReviewReactions
            .AnyAsync(rr => rr.ReviewId == review.Id
                && rr.UserId == _currentUser.UserId
                && rr.Type == ReactionType.Like,
                cancellationToken);

        return new ReviewResponse(
            review.Id,
            review.UserId,
            username,
            movieTitle,
            review.Rating.Value,
            review.ReviewText,
            review.ContainsSpoilers,
            review.LikesCount,
            isLiked,
            review.CreatedAt);
    }
}
