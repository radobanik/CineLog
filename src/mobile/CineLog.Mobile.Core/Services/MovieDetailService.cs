using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Movies;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class MovieDetailService(IMoviesClient moviesClient) : IMovieDetailService
{
    public async Task<MovieDetailInfo> GetMovieDetailAsync(Guid id, CancellationToken ct = default)
    {
        var response = await moviesClient.GetByIdAsync(id, ct);

        var director = response.Crew?
            .FirstOrDefault(c => string.Equals(c.Job, "Director", StringComparison.OrdinalIgnoreCase))
            ?.Name;

        var subtitle = BuildSubtitle(response.ReleaseDate, response.RuntimeMinutes);
        var ratingsCountText = BuildRatingsCountText(response.RatingsCount);

        var cast = (response.Cast ?? [])
            .Select(c => new CastMemberItem
            {
                Id = c.PersonId ?? Guid.Empty,
                Name = c.Name ?? string.Empty,
                Character = c.Character,
                ProfilePath = c.ProfilePath,
            })
            .ToList();

        return new MovieDetailInfo
        {
            Id = response.Id ?? id,
            Title = response.Title ?? string.Empty,
            Subtitle = subtitle,
            Director = director is not null ? $"Directed by {director}" : null,
            PosterPath = response.PosterPath,
            BackdropPath = response.BackdropPath,
            Overview = response.Overview,
            AverageRating = response.AverageRating,
            RatingsCountText = ratingsCountText,
            Cast = cast,
        };
    }

    public async Task<(IReadOnlyList<ReviewPreviewItem> Items, int TotalCount)> GetReviewsAsync(
        Guid movieId, int count, CancellationToken ct = default)
    {
        var response = await moviesClient.GetReviewsAsync(movieId, 1, count, ct);
        var items = (response.Items ?? []).Select(MapReview).ToList();
        return (items, response.TotalCount ?? 0);
    }

    public async Task<(IReadOnlyList<ReviewListItem> Items, int TotalCount, int TotalPages)> GetReviewsPageAsync(
        Guid movieId, int page, int pageSize, CancellationToken ct = default)
    {
        var response = await moviesClient.GetReviewsAsync(movieId, page, pageSize, ct);
        var items = (response.Items ?? []).Select(MapReviewListItem).ToList();
        return (items, response.TotalCount ?? 0, response.TotalPages ?? 0);
    }

    private static ReviewListItem MapReviewListItem(ReviewResponse r) => new()
    {
        Id = r.Id ?? Guid.Empty,
        Username = r.Username ?? string.Empty,
        Rating = r.Rating,
        ReviewText = r.ReviewText,
        LikesCount = r.LikesCount ?? 0,
        IsLiked = r.IsLiked ?? false,
        CreatedAt = r.CreatedAt,
    };

    private static ReviewPreviewItem MapReview(ReviewResponse r) => new()
    {
        Id = r.Id ?? Guid.Empty,
        Username = r.Username ?? string.Empty,
        Rating = r.Rating,
        ReviewText = r.ReviewText,
        LikesCount = r.LikesCount ?? 0,
        IsLiked = r.IsLiked ?? false,
        CreatedAt = r.CreatedAt,
    };

    private static string BuildSubtitle(DateTimeOffset? releaseDate, int? runtimeMinutes)
    {
        var parts = new List<string>();

        if (releaseDate.HasValue)
            parts.Add(releaseDate.Value.Year.ToString());

        if (runtimeMinutes.HasValue && runtimeMinutes > 0)
        {
            var h = runtimeMinutes.Value / 60;
            var m = runtimeMinutes.Value % 60;
            parts.Add(h > 0 ? $"{h}h {m}m" : $"{m}m");
        }

        return string.Join(" · ", parts);
    }

    private static string BuildRatingsCountText(int? count)
    {
        if (!count.HasValue || count.Value == 0)
            return string.Empty;

        var formatted = count.Value >= 1000
            ? $"{count.Value / 1000.0:0.#}k"
            : count.Value.ToString();

        return $"based on {formatted} ratings on IMDb";
    }
}
