using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.WatchList;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CineLog.Mobile.Core.ViewModels.WatchList.helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;



namespace CineLog.Mobile.Core.ViewModels.WatchList
{
    public partial class WatchListMoviesViewModel(IWatchListService watchListService, IAlertService alerts) : BaseViewModel(alerts)
    {
        private readonly MoviePageLoader _pageLoader = new();

        [ObservableProperty] private WatchListCollectionItem? _selectedWatchList;
        [ObservableProperty] private WatchListRowViewModel? _selectedRow;
        [ObservableProperty] private bool _hasMovies;
        [ObservableProperty] private bool _canLoadMore;
        [ObservableProperty] private bool _isLoadingMore;

        public ObservableCollection<MovieItem> Movies { get; } = [];

        public async Task OpenAsync(WatchListRowViewModel row)
        {
            SelectedRow = row;
            SelectedWatchList = row.Item;
            Title = row.Name;

            Movies.Clear();

            var movies = await watchListService.GetMoviesAsync(row.Item);
            _pageLoader.Reset(movies);

            LoadNextMoviePage();
        }

        public void Clear()
        {
            SelectedRow = null;
            SelectedWatchList = null;
            Title = string.Empty;

            Movies.Clear();
            _pageLoader.Clear();

            HasMovies = false;
            CanLoadMore = false;
        }

        [RelayCommand]
        private async Task RemoveMovie(MovieItem? movie)
        {
            if (movie is null || SelectedWatchList is null)
                return;

            await ExecuteAsync(async () =>
            {
                await watchListService.RemoveMovieFromWatchListAsync(SelectedWatchList, movie.Id);

                _pageLoader.Remove(movie.Id);
                Movies.Remove(movie);

                if (SelectedRow is not null)
                    SelectedRow.ItemCount = Math.Max(0, SelectedRow.ItemCount - 1);

                HasMovies = Movies.Count > 0;
                CanLoadMore = _pageLoader.CanLoadMore;

                await alerts.ShowToastAsync(
                    SelectedWatchList.IsFavorites
                        ? "Removed from favorites."
                        : "Movie removed from list.");
            });
        }

        [RelayCommand]
        private Task LoadMore()
        {
            if (IsLoadingMore || !CanLoadMore)
                return Task.CompletedTask;

            IsLoadingMore = true;
            LoadNextMoviePage();
            IsLoadingMore = false;

            return Task.CompletedTask;
        }

        private void LoadNextMoviePage()
        {
            foreach (var movie in _pageLoader.LoadNextPage())
                Movies.Add(movie);

            HasMovies = Movies.Count > 0;
            CanLoadMore = _pageLoader.CanLoadMore;
        }
    }

}
