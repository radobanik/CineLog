using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Services.Interfaces;
using Microsoft.Extensions.Options;

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

        public async Task<IReadOnlyList<ReviewItem>> GetRecentReviewsAsync(int count, CancellationToken ct = default)
        {
            var actions = await _dashboardClient.NewActionsAsync(count, ct);
            return MapReviews(actions);

           
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

        private static IReadOnlyList<ReviewItem> MapReviews(IEnumerable<NewActionResponse> reviews)
        {
            return [.. reviews
                .Where(review => review.Review is not null && review.Movie is not null && review.User is not null)
                .Select(review => new ReviewItem
                {
                    ReviewId = review.Review!.Id ?? Guid.Empty,
                    MovieId = review.Movie!.Id,
                    Username = review.User!.Username ?? string.Empty,
                    AvatarUrl = review.User!.AvatarUrl,
                    MovieTitle = review.Movie!.Title ?? string.Empty,
                    PosterPath = review.Movie!.PosterPath,
                    ReviewText = review.Review!.ReviewText,
                    Rating = review.Review!.Rating,
                    ContainsSpoilers = review.Review!.ContainsSpoilers ?? false,
                    LikesCount = review.Review!.LikesCount ?? 0,
                    CreatedAt = review.Review!.CreatedAt
                })];
        }
    }
}
