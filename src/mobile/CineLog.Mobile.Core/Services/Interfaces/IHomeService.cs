using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.Core.Models.Home;

namespace CineLog.Mobile.Core.Services.Interfaces
{
    public interface IHomeService
    {
        Task<IReadOnlyList<HomeMovieItem>> GetTopRatedMoviesAsync(int count, CancellationToken ct = default);
        Task<IReadOnlyList<HomeMovieItem>> GetNewReleaseMoviesAsync(int count, CancellationToken ct = default);
    }
}
