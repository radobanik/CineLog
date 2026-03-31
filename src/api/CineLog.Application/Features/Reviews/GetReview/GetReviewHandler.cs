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

    public GetReviewHandler(IReviewRepository reviewRepository, IAppDbContext context)
    {
        _reviewRepository = reviewRepository;
        _context = context;
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

        return new ReviewResponse(
            review.Id,
            review.UserId,
            username,
            movieTitle,
            review.Rating.Value,
            review.ReviewText,
            review.ContainsSpoilers,
            review.LikesCount,
            review.CreatedAt);
    }
}
