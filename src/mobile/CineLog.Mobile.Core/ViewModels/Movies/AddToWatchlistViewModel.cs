using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Infrastructure;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CineLog.Mobile.Core.ViewModels.WatchList;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Movies;

public partial class AddToWatchlistViewModel(
    IWatchListService watchListService,
    IMovieDetailNavigationContext movieDetailNav,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    [ObservableProperty]
    private bool _hasWatchlists;

    public ObservableCollection<WatchListRowViewModel> Lists { get; } = [];

    protected override async Task LoadAsync()
    {
        Title = "Add to Watchlist";

        Lists.Clear();

        foreach (var watchList in await watchListService.GetWatchListsAsync())
            Lists.Add(new WatchListRowViewModel(watchList));

        HasWatchlists = Lists.Count > 0;
    }

    [RelayCommand]
    private async Task AddToWatchlist(WatchListRowViewModel? row)
    {
        if (row is null)
            return;

        IsBusy = true;

        try
        {
            await watchListService.AddMovieToWatchListAsync(row.Item, movieDetailNav.MovieId);
            await alerts.ShowToastAsync($"Added to {row.Name}.");
            await navigation.NavigateBackAsync();
        }
        catch (ApiException ex) when (ex.StatusCode == 409)
        {
            await alerts.ShowToastAsync("Already in this watchlist.");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
