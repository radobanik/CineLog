using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.WatchList;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CineLog.Mobile.Core.ViewModels.WatchList;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Movies;

public partial class AddToWatchlistViewModel(
    IWatchlistsClient watchlistsClient,
    IMovieDetailNavigationContext movieDetailNav,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    [ObservableProperty] private bool _hasWatchlists;

    public ObservableCollection<WatchListRowViewModel> Lists { get; } = [];

    protected override async Task LoadAsync()
    {
        Title = "Add to Watchlist";
        var watchlists = await watchlistsClient.GetAllAsync();

        Lists.Clear();
        foreach (var w in watchlists)
            Lists.Add(new WatchListRowViewModel(new WatchListCollectionItem
            {
                Id = w.Id ?? Guid.Empty,
                Name = w.Name ?? "Untitled list",
                ItemCount = w.ItemCount ?? 0,
            }));

        HasWatchlists = Lists.Count > 0;
    }

    [RelayCommand]
    private async Task AddToWatchlist(WatchListRowViewModel? row)
    {
        if (row is null) return;

        await ExecuteAsync(async () =>
        {
            await watchlistsClient.AddMovieAsync(row.Id, movieDetailNav.MovieId);
            await alerts.ShowToastAsync($"Added to {row.Name}.");
        });
    }

    [RelayCommand]
    private Task GoBack() => navigation.NavigateBackAsync();
}
