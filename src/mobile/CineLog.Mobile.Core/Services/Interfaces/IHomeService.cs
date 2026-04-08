using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.Core.Models.Home;

namespace CineLog.Mobile.Core.Services.Interfaces
{
    public interface IHomeService
    {
        Task<IReadOnlyList<HomeMovieItem>> GetMoviesAsync(HomeCategory category, CancellationToken ct = default);
    }
}
