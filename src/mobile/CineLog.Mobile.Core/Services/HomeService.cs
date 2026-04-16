using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class HomeService(IDashboardClient dashboardClient) : IHomeService
{
    public async Task<IReadOnlyList<MovieItem>> GetTopRatedMoviesAsync(int count, CancellationToken ct = default)
    {
        var movies = await dashboardClient.TopRatedMoviesAsync(count, ct);
        return MapMovies(movies);
    }

    public async Task<IReadOnlyList<MovieItem>> GetNewReleaseMoviesAsync(int count, CancellationToken ct = default)
    {
        var movies = await dashboardClient.NewestMoviesAsync(count, ct);
        return MapMovies(movies);
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
