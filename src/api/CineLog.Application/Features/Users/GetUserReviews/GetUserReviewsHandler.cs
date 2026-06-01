using CineLog.Application.Common;
using CineLog.Application.Features.Reviews;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Users.GetUserReviews;

public class GetUserReviewsHandler : IRequestHandler<GetUserReviewsQuery, PagedResponse<ReviewResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUserReviewsHandler(IReviewRepository reviewRepository, IAppDbContext context, ICurrentUserService currentUser)
    {
        _reviewRepository = reviewRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PagedResponse<ReviewResponse>> Handle(
        GetUserReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await _context.Reviews
            .CountAsync(r => r.UserId == request.UserId, cancellationToken);

        var reviews = await _reviewRepository.GetByUserIdAsync(
            request.UserId, request.Page, request.PageSize, cancellationToken);

        var movieIds = reviews.Select(r => r.MovieId).Distinct().ToList();
        var movies = await _context.Movies
            .Where(m => movieIds.Contains(m.Id))
            .Select(m => new { m.Id, m.Title, m.PosterPath })
            .ToListAsync(cancellationToken);

        var userInfo = await _context.Users
            .Where(u => u.Id == request.UserId)
            .Select(u => new { u.UserName, u.AvatarUrl })
            .FirstOrDefaultAsync(cancellationToken);

        var username = userInfo?.UserName ?? string.Empty;
        var avatarUrl = userInfo?.AvatarUrl;

        var reviewIds = reviews.Select(r => r.Id).ToList();
        var likedIds = await _context.ReviewReactions
            .Where(rr => rr.UserId == _currentUser.UserId
                && reviewIds.Contains(rr.ReviewId)
                && rr.Type == ReactionType.Like)
            .Select(rr => rr.ReviewId)
            .ToHashSetAsync(cancellationToken);

        var movieTitleMap = movies.ToDictionary(m => m.Id, m => m.Title);
        var moviePosterMap = movies.ToDictionary(m => m.Id, m => m.PosterPath);

        var items = reviews.Select(r => new ReviewResponse(
            r.Id,
            r.UserId,
            username,
            avatarUrl,
            r.MovieId,
            movieTitleMap.GetValueOrDefault(r.MovieId, string.Empty),
            moviePosterMap.GetValueOrDefault(r.MovieId),
            r.Rating.Value,
            r.ReviewText,
            r.ContainsSpoilers,
            r.LikesCount,
            likedIds.Contains(r.Id),
            r.CreatedAt)).ToList();

        return PagedResponse<ReviewResponse>.Create(items, request.Page, request.PageSize, totalCount);
    }
}
