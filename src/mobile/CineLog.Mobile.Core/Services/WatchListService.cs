using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.WatchList;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services
{
    public class WatchListService : IWatchListService
    {
        private readonly IWatchlistsClient _watchlistsClient;
        private readonly IUsersClient _usersClient;
        private readonly IMoviesClient _moviesClient;

        public WatchListService(IWatchlistsClient watchlistsClient, IUsersClient usersClient, IMoviesClient moviesClient)
        {
            _watchlistsClient = watchlistsClient;
            _usersClient = usersClient;
            _moviesClient = moviesClient;
        }


        public async Task<IReadOnlyList<WatchListCollectionItem>> GetWatchListsAsync(CancellationToken ct = default)
        {
            {
                var favorites = await _usersClient.GetFavoritesAsync(ct);
                var watchlists = await _watchlistsClient.GetAllAsync(ct);

                var result = new List<WatchListCollectionItem>
                {
                    new()
                    {
                        Id = Guid.Empty,
                        Name = "Favorites",
                        ItemCount = favorites.Count,
                        IsFavorites = true
                    }
                };

                result.AddRange(watchlists.Select(w => new WatchListCollectionItem
                {
                    Id = w.Id ?? Guid.Empty,
                    Name = w.Name ?? "Untitled list",
                    ItemCount = w.ItemCount ?? 0,
                }));

                return result;
            }
        }

        public async Task<IReadOnlyList<MovieItem>> GetMoviesAsync(WatchListCollectionItem watchList, CancellationToken ct = default)
        {
            ICollection<MovieListItemResponse> movies = watchList.IsFavorites
                ? await _usersClient.GetFavoritesAsync(ct)
                : (await _watchlistsClient.GetByIdAsync(watchList.Id, ct)).Movies ?? [];

            return [.. movies.Select(movie => MapMovie(movie, watchList.IsFavorites))];
        }

        public Task<Guid> CreateWatchListAsync(string name, CancellationToken ct = default)
        {
            return _watchlistsClient.CreateAsync(new CreateWatchlistCommand { Name = name }, ct);
        }

        public Task DeleteWatchListAsync(WatchListCollectionItem watchList, CancellationToken ct = default)
        {
            if (watchList.IsFavorites)
                return Task.CompletedTask;

            return _watchlistsClient.DeleteAsync(watchList.Id, ct);
        }

        public Task RemoveMovieFromWatchListAsync(WatchListCollectionItem watchList, Guid movieId, CancellationToken ct = default)
        {
            return watchList.IsFavorites
                ? _moviesClient.RemoveFromFavoritesAsync(movieId, ct)
                : _watchlistsClient.RemoveMovieAsync(watchList.Id, movieId, ct);
        }

        private static MovieItem MapMovie(MovieListItemResponse movie, bool isFavorite) => new()
        {
            Id = movie.Id ?? Guid.Empty,
            Title = movie.Title ?? string.Empty,
            PosterPath = movie.PosterPath,
            AverageRating = movie.AverageRating,
            IsFavorite = isFavorite
        };
    }
}
