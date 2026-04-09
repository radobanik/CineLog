using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services
{
    public sealed class HomeService : IHomeService
    {
        private const MovieType movieContentType = MovieType._0; // _1 = Series

        private readonly IDashboardClient _dashboardClient;

        public HomeService(IDashboardClient dashboardClient)
        {
            _dashboardClient = dashboardClient;
        }

        public async Task<IReadOnlyList<HomeMovieItem>> GetTopRatedMoviesAsync(int count, CancellationToken ct = default)
        {
            var topRatedMovies = await _dashboardClient.TopRatedMoviesAsync(count, ct);
            return MapMovies(topRatedMovies);
        }

        public async Task<IReadOnlyList<HomeMovieItem>> GetNewReleaseMoviesAsync(int count, CancellationToken ct = default)
        {
            var newReleaseMovies = await _dashboardClient.NewestMoviesAsync(count, ct);
            return MapMovies(newReleaseMovies);
        }

        private static IReadOnlyList<HomeMovieItem> MapMovies(IEnumerable<MovieSummaryResponse> movies)
        {
            return [.. movies
                //.Where(movie => movie.Type == movieContentType)
                .Select(movie => new HomeMovieItem
                {
                    Id = movie.Id ?? Guid.Empty,
                    Title = movie.Title ?? string.Empty,
                    PosterPath = movie.PosterPath,
                    AverageRating = movie.AverageRating
                })];
        }
    }
}
