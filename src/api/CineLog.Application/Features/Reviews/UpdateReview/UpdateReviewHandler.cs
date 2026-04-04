using CineLog.Application.Common;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using CineLog.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Reviews.UpdateReview;

public class UpdateReviewHandler : IRequestHandler<UpdateReviewCommand, ReviewResponse>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IAppDbContext _context;

    public UpdateReviewHandler(
        IReviewRepository reviewRepository,
        IMovieRepository movieRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        IAppDbContext context)
    {
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<ReviewResponse> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken)
            ?? throw new NotFoundException($"Review {request.ReviewId} not found.");

        if (review.UserId != _currentUser.UserId)
            throw new UnauthorizedAccessException("You are not the author of this review.");

        var user = await _userRepository.GetByIdAsync(review.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {review.UserId} not found.");

        var movie = await _movieRepository.GetByIdAsync(review.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {review.MovieId} not found.");

        review.Update(Rating.Create(request.Rating), request.ReviewText, request.ContainsSpoilers);
        await _reviewRepository.UpdateAsync(review, cancellationToken);

        var isLiked = await _context.ReviewReactions
            .AnyAsync(rr => rr.ReviewId == review.Id
                && rr.UserId == _currentUser.UserId
                && rr.Type == ReactionType.Like,
                cancellationToken);

        return new ReviewResponse(
            review.Id,
            review.UserId,
            user.UserName!,
            movie.Title,
            review.Rating.Value,
            review.ReviewText,
            review.ContainsSpoilers,
            review.LikesCount,
            isLiked,
            review.CreatedAt);
    }
}
