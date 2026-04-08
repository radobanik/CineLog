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
        private readonly IDashboardClient _dashboardClient;
        private const MovieType movieContentType = MovieType._0; // _1 = Series

        public HomeService(IDashboardClient dashboardClient)
        {
            _dashboardClient = dashboardClient;
        }

        public async Task<IReadOnlyList<HomeMovieItem>> GetMoviesAsync(HomeCategory category, CancellationToken ct = default)
        {
            ICollection<MovieSummaryResponse> movies = category switch
            {
                HomeCategory.HighestRated => await _dashboardClient.TopRatedMoviesAsync(24, ct),
                HomeCategory.NewReleases => await _dashboardClient.NewestMoviesAsync(24, ct),

                // Not supported by the current backend/mobile client yet.
                //HomeCategory.Popular => [],
                //HomeCategory.NowPlaying => [],
                //HomeCategory.Upcoming => [],

                _ => []
            };

            return [.. movies
                .Where(movie => movie.Type == movieContentType)
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
