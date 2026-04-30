using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.WatchList;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.WatchList;

public partial class WatchListViewModel(
    IWatchListService watchListService,
    IAlertService alerts) : BaseViewModel(alerts)
{
    private const int MoviePageSize = 12;

    private readonly List<MovieItem> _allMovies = [];
    private int _currentPage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowWatchLists))]
    [NotifyPropertyChangedFor(nameof(ShowMovies))]
    private WatchListCollectionItem? _selectedWatchList;

    [ObservableProperty] private string _newWatchListName = string.Empty;
    [ObservableProperty] private bool _hasWatchLists;
    [ObservableProperty] private bool _hasMovies;
    [ObservableProperty] private bool _canLoadMore;
    [ObservableProperty] private bool _isLoadingMore;
    [ObservableProperty] private bool _isCreatingWatchList;
    [ObservableProperty] private string _editingWatchListName = string.Empty;
    [ObservableProperty] private WatchListRowViewModel? _watchListBeingEdited;

    public bool ShowWatchLists => SelectedWatchList is null;
    public bool ShowMovies => SelectedWatchList is not null;

    public ObservableCollection<WatchListRowViewModel> Lists { get; } = [];
    public ObservableCollection<MovieItem> Movies { get; } = [];

    protected override async Task LoadAsync()
    {
        Title = "WatchLists";
        await ReloadWatchListsAsync();
    }

    [RelayCommand]
    private async Task Load() => await ExecuteAsync(ReloadWatchListsAsync);

    [RelayCommand]
    private void ShowCreateWatchList()
    {
        CloseOptions();
        NewWatchListName = string.Empty;
        IsCreatingWatchList = true;
    }

    [RelayCommand]
    private void CancelCreateWatchList()
    {
        NewWatchListName = string.Empty;
        IsCreatingWatchList = false;
    }

    [RelayCommand]
    private async Task ConfirmCreateWatchList()
    {
        var name = NewWatchListName.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return;

        await ExecuteAsync(async () =>
        {
            await watchListService.CreateWatchListAsync(name);
            NewWatchListName = string.Empty;
            IsCreatingWatchList = false;
            await ReloadWatchListsAsync();
        });
    }

    [RelayCommand]
    private async Task OpenWatchList(WatchListRowViewModel? row)
    {
        if (row is null)
            return;

        CloseOptions();

        await ExecuteAsync(async () =>
        {
            SelectedWatchList = row.Item;
            Title = row.Name;

            _allMovies.Clear();
            _allMovies.AddRange(await watchListService.GetMoviesAsync(row.Item));

            Movies.Clear();
            _currentPage = 0;
            LoadNextMoviePage();
        });
    }

    [RelayCommand]
    private void BackToLists()
    {
        SelectedWatchList = null;
        Title = "WatchLists";

        Movies.Clear();
        _allMovies.Clear();

        HasMovies = false;
        CanLoadMore = false;
    }

    [RelayCommand]
    private void ToggleWatchListOptions(WatchListRowViewModel? row)
    {
        if (row is null)
            return;

        foreach (var item in Lists)
            item.IsOptionsOpen = item == row && !item.IsOptionsOpen;
    }

    [RelayCommand]
    private void StartEditWatchList(WatchListRowViewModel? row)
    {
        if (row is null || !row.CanEdit)
            return;

        CloseOptions();

        WatchListBeingEdited = row;
        EditingWatchListName = row.Name;
    }

    [RelayCommand]
    private async Task ConfirmEditWatchList()
    {
        if (WatchListBeingEdited is null)
            return;

        await alerts.ShowAlertAsync(
            "Not implemented",
            "Renaming watchlists needs a backend endpoint first.");

        WatchListBeingEdited = null;
        EditingWatchListName = string.Empty;
    }

    [RelayCommand]
    private void CancelEditWatchList()
    {
        WatchListBeingEdited = null;
        EditingWatchListName = string.Empty;
    }

    [RelayCommand]
    private async Task DeleteWatchList(WatchListRowViewModel? row)
    {
        if (row is null || !row.CanDelete)
            return;

        CloseOptions();

        await ExecuteAsync(async () =>
        {
            await watchListService.DeleteWatchListAsync(row.Item);
            Lists.Remove(row);

            if (SelectedWatchList?.Id == row.Id)
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

    private async Task ReloadWatchListsAsync()
    {
        Lists.Clear();

        foreach (var list in await watchListService.GetWatchListsAsync())
            Lists.Add(new WatchListRowViewModel(list));

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
        HasMovies = Movies.Count > 0;
        CanLoadMore = Movies.Count < _allMovies.Count;
    }

    private void CloseOptions()
    {
        foreach (var item in Lists)
            item.IsOptionsOpen = false;
    }
}
