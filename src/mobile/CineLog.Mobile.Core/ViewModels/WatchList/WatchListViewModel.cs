using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.WatchList;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.WatchList
{
    public partial class WatchListViewModel(IWatchListService watchListService, IAlertService alerts) : BaseViewModel(alerts)
    {
        private const int MoviePageSize = 12;
        private int _currentPage;
   
        private readonly List<MovieItem> _allMovies = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowWatchLists))]
        [NotifyPropertyChangedFor(nameof(ShowMovies))]
        private WatchListCollectionItem? _selectedWatchList;
        public bool ShowWatchLists => SelectedWatchList is null;
        public bool ShowMovies => SelectedWatchList is not null;


        [ObservableProperty] private string _newWatchListName = string.Empty;
        [ObservableProperty] private bool _hasWatchLists;
        [ObservableProperty] private bool _hasMovies;
        [ObservableProperty] private bool _canLoadMore;
        [ObservableProperty] private bool _isLoadingMore;
        [ObservableProperty] private bool _isCreatingList;

        public ObservableCollection<WatchListCollectionItem> Lists { get; } = [];
        public ObservableCollection<MovieItem> Movies { get; } = [];

        protected override async Task LoadAsync()
        {
            Title = "WatchLists";
            await ReloadWatchListsAsync();
        }

        [RelayCommand]
        private async Task Load() => await ExecuteAsync(ReloadWatchListsAsync);

        [RelayCommand]
        private async Task CreateWatchList()
        {
            var name = NewWatchListName.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return;

            await ExecuteAsync(async () =>
            {
                IsCreatingList = true;
                await watchListService.CreateWatchListAsync(name);
                NewWatchListName = string.Empty;
                await ReloadWatchListsAsync();
            });

            IsCreatingList = false;
        }

        [RelayCommand]
        private async Task OpenWatchList(WatchListCollectionItem? watchList)
        {
            if (watchList is null)
                return;

            await ExecuteAsync(async () =>
            {
                SelectedWatchList = watchList;
                Title = watchList.Name;

                _allMovies.Clear();
                _allMovies.AddRange(await watchListService.GetMoviesAsync(watchList));

                Movies.Clear();
                _currentPage = 0;
                LoadNextMoviePage();

                HasMovies = Movies.Count > 0;
            });
        }

        [RelayCommand]
        private void BackToLists()
        {
            SelectedWatchList = null;
            Title = "Lists";
            Movies.Clear();
            _allMovies.Clear();
            HasMovies = false;
            CanLoadMore = false;
        }

        [RelayCommand]
        private async Task DeleteWatchList(WatchListCollectionItem? watchList)
        {
            if (watchList is null || !watchList.CanDelete)
                return;

            await ExecuteAsync(async () =>
            {
                await watchListService.DeleteWatchListAsync(watchList);
                Lists.Remove(watchList);

                if (SelectedWatchList?.Id == watchList.Id)
                    BackToLists();

                HasWatchLists = Lists.Count > 0;
                await alerts.ShowToastAsync("List deleted.");
            });
        }

        [RelayCommand]
        private async Task RemoveMovie(MovieItem? movie)
        {
            if (movie is null || SelectedWatchList is null)
                return;

            await ExecuteAsync(async () =>
            {
                await watchListService.RemoveMovieFromWatchListAsync(SelectedWatchList, movie.Id);

                _allMovies.RemoveAll(x => x.Id == movie.Id);
                Movies.Remove(movie);

                SelectedWatchList.ItemCount = Math.Max(0, SelectedWatchList.ItemCount - 1);
                HasMovies = Movies.Count > 0;
                CanLoadMore = Movies.Count < _allMovies.Count;

                await alerts.ShowToastAsync("Movie removed.");
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

        private async Task ReloadWatchListsAsync()
        {
            Lists.Clear();

            foreach (var list in await watchListService.GetWatchListsAsync())
                Lists.Add(list);

            HasWatchLists = Lists.Count > 0;
        }

        private void LoadNextMoviePage()
        {
            var nextMovies = _allMovies
                .Skip(_currentPage * MoviePageSize)
                .Take(MoviePageSize)
                .ToList();

            foreach (var movie in nextMovies)
                Movies.Add(movie);

            _currentPage++;
            CanLoadMore = Movies.Count < _allMovies.Count;
            HasMovies = Movies.Count > 0;
        }
    }
}
