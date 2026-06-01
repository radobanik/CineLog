using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class HomeService(IDashboardClient dashboardClient) : IHomeService
{
    public async Task<IReadOnlyList<MovieItem>> GetTopRatedMoviesAsync(int count, CancellationToken ct = default)
    {
        var movies = await dashboardClient.GetTopRatedAsync(count, ct);
        return MapMovies(movies);
    }

    public async Task<IReadOnlyList<MovieItem>> GetNewReleaseMoviesAsync(int count, CancellationToken ct = default)
    {
        var movies = await dashboardClient.GetNewestAsync(count, ct);
        return MapMovies(movies);
    }

    public async Task<IReadOnlyList<ReviewListItem>> GetLatestReviewsAsync(int count, CancellationToken ct = default)
    {
        var actions = await dashboardClient.GetNewActionsAsync(count, ct);

        return [.. actions
        .Where(action => action.Review is not null)
        .Select(action => new ReviewListItem
        {
            Id = action.Review?.Id ?? Guid.Empty,
            MovieId = action.Movie?.Id ?? Guid.Empty,
            Username = action.User?.Username ?? string.Empty,
            AvatarUrl = action.User?.AvatarUrl,
            MovieTitle = action.Movie?.Title ?? string.Empty,
            MoviePosterPath = action.Movie?.PosterPath,
            Rating = action.Review?.Rating,
            ReviewText = action.Review?.ReviewText,
            LikesCount = action.Review?.LikesCount ?? 0,
            CreatedAt = action.Review?.CreatedAt
        })];
    }

    private static IReadOnlyList<MovieItem> MapMovies(IEnumerable<MovieSummaryResponse> movies) =>
        [.. movies.Select(m => new MovieItem
        {
            Id = m.Id ?? Guid.Empty,
            Title = m.Title ?? string.Empty,
            PosterPath = m.PosterPath,
            AverageRating = m.AverageRating
        })];
}
