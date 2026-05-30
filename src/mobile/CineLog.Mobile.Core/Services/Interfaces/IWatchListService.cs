using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.WatchList;

namespace CineLog.Mobile.Core.Services.Interfaces
{
    public interface IWatchListService
    {
        Task<IReadOnlyList<WatchListCollectionItem>> GetWatchListsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<MovieItem>> GetMoviesAsync(WatchListCollectionItem list, CancellationToken ct = default);
        Task<Guid> CreateWatchListAsync(string name, CancellationToken ct = default);
        Task DeleteWatchListAsync(WatchListCollectionItem watchList, CancellationToken ct = default);
        Task AddMovieToWatchListAsync(WatchListCollectionItem watchList, Guid movieId, CancellationToken ct = default);
        Task RemoveMovieFromWatchListAsync(WatchListCollectionItem list, Guid movieId, CancellationToken ct = default);
    }
}
