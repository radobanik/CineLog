using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Dashboard.GetNewActions;

public class GetNewActionsHandler : IRequestHandler<GetNewActionsQuery, List<NewActionResponse>>
{
    private readonly IAppDbContext _db;

    public GetNewActionsHandler(IAppDbContext db) => _db = db;

    public async Task<List<NewActionResponse>> Handle(
        GetNewActionsQuery request,
        CancellationToken cancellationToken)
    {
        var results = await _db.Reviews
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .Take(request.Count)
            .Join(_db.Movies, r => r.MovieId, m => m.Id, (r, m) => new { Review = r, Movie = m })
            .Join(_db.Users, x => x.Review.UserId, u => u.Id, (x, u) => new { x.Review, x.Movie, User = u })
            .ToListAsync(cancellationToken);

        return results.Select(x => new NewActionResponse(
            new ReviewInfo(
                x.Review.Id,
                x.Review.Rating.Value,
                x.Review.ReviewText,
                x.Review.ContainsSpoilers,
                x.Review.WatchedOn,
                x.Review.LikesCount,
                x.Review.CreatedAt),
            new MovieInfo(x.Movie.Id, x.Movie.Title, x.Movie.PosterPath),
            new UserInfo(x.User.Id, x.User.UserName!, x.User.AvatarUrl)
        )).ToList();
    }
}
