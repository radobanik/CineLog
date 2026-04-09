using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Models.Review;

namespace CineLog.Mobile.Core.Services.Interfaces
{
    public interface IHomeService
    {
        Task<IReadOnlyList<HomeMovieItem>> GetTopRatedMoviesAsync(int count, CancellationToken ct = default);
        Task<IReadOnlyList<HomeMovieItem>> GetNewReleaseMoviesAsync(int count, CancellationToken ct = default)
        Task<IReadOnlyList<ReviewItem>> GetRecentReviewsAsync(int count, CancellationToken ct = default);

    }
}
