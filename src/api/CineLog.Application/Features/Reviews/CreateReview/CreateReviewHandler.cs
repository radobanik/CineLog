using CineLog.Application.Common;
using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using CineLog.Domain.ValueObjects;
using MediatR;

namespace CineLog.Application.Features.Reviews.CreateReview;

public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, ReviewResponse>
{
    private readonly IAppDbContext _context;
    private readonly IMovieRepository _movieRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPublisher _publisher;

    public CreateReviewHandler(
        IAppDbContext context,
        IMovieRepository movieRepository,
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser,
        IPublisher publisher)
    {
        _context = context;
        _movieRepository = movieRepository;
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public async Task<ReviewResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {_currentUser.UserId} not found.");

        var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        var existing = await _context.Reviews
            .Where(r => r.UserId == _currentUser.UserId && r.MovieId == request.MovieId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
            throw new ConflictException("You have already reviewed this movie.");

        var review = Review.Create(
            _currentUser.UserId,
            request.MovieId,
            Rating.Create(request.Rating),
            request.ReviewText,
            request.ContainsSpoilers);

        await _reviewRepository.AddAsync(review, cancellationToken);

        var allReviews = await _context.Reviews
            .Where(r => r.MovieId == request.MovieId)
            .ToListAsync(cancellationToken);

        var avgRating = allReviews.Average(r => r.Rating.Value);
        movie.UpdateAverageRating(avgRating, allReviews.Count);
        await _movieRepository.UpdateAsync(movie, cancellationToken);

        foreach (var domainEvent in review.DomainEvents)
            await _publisher.Publish(domainEvent, cancellationToken);

        review.ClearDomainEvents();

        return new ReviewResponse(
            review.Id,
            review.UserId,
            user.UserName!,
            movie.Title,
            review.Rating.Value,
            review.ReviewText,
            review.ContainsSpoilers,
            review.LikesCount,
            review.CreatedAt);
    }
}
