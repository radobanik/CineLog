using CineLog.Application.Common;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Reviews.GetMovieReviews;

public class GetMovieReviewsHandler : IRequestHandler<GetMovieReviewsQuery, PagedResponse<ReviewResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMovieReviewsHandler(IReviewRepository reviewRepository, IAppDbContext context, ICurrentUserService currentUser)
    {
        _reviewRepository = reviewRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResponse<ReviewResponse>> Handle(
        GetMovieReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await _context.Reviews
            .CountAsync(r => r.MovieId == request.MovieId, cancellationToken);

        var reviews = await _reviewRepository.GetByMovieIdAsync(
            request.MovieId, request.Page, request.PageSize, cancellationToken);

        var userIds = reviews.Select(r => r.UserId).Distinct().ToList();
        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName, u.AvatarUrl })
            .ToListAsync(cancellationToken);

        var movieInfo = await _context.Movies
            .Where(m => m.Id == request.MovieId)
            .Select(m => new { m.Title, m.PosterPath })
            .FirstOrDefaultAsync(cancellationToken);
        var movieTitle = movieInfo?.Title ?? string.Empty;
        var moviePosterPath = movieInfo?.PosterPath;

        var reviewIds = reviews.Select(r => r.Id).ToList();
        var likedIds = await _context.ReviewReactions
            .Where(rr => rr.UserId == _currentUser.UserId
                && reviewIds.Contains(rr.ReviewId)
                && rr.Type == ReactionType.Like)
            .Select(rr => rr.ReviewId)
            .ToHashSetAsync(cancellationToken);

        var usernameMap = users.ToDictionary(u => u.Id, u => u.UserName ?? string.Empty);
        var avatarMap = users.ToDictionary(u => u.Id, u => u.AvatarUrl);

        var items = reviews.Select(r => new ReviewResponse(
            r.Id,
            r.UserId,
            usernameMap.GetValueOrDefault(r.UserId, string.Empty),
            avatarMap.GetValueOrDefault(r.UserId),
            movieTitle,
            moviePosterPath,
            r.Rating.Value,
            r.ReviewText,
            r.ContainsSpoilers,
            r.LikesCount,
            likedIds.Contains(r.Id),
            r.CreatedAt)).ToList();

        return PagedResponse<ReviewResponse>.Create(items, request.Page, request.PageSize, totalCount);
    }
}
