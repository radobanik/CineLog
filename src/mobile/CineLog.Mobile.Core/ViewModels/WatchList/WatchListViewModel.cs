using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.WatchList;

public partial class WatchListViewModel(
    IWatchListService watchListService,
    IAlertService alerts,
    WatchListMoviesViewModel moviesViewModel) : BaseViewModel(alerts)
{
    private WatchListRowViewModel? _openOptionsRow;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowWatchLists))]
    [NotifyPropertyChangedFor(nameof(ShowMovies))]
    private bool _isViewingMovies;

    public bool ShowWatchLists => !IsViewingMovies;
    public bool ShowMovies => IsViewingMovies;
    public bool HasWatchLists => Lists.Count > 0;

    public WatchListNameFormViewModel NameForm { get; } = new();
    public WatchListMoviesViewModel MoviesViewModel { get; } = moviesViewModel;
    public ObservableCollection<WatchListRowViewModel> Lists { get; } = [];

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
        SetOptionsRow(null);
        NameForm.BeginCreate();
    }

    [RelayCommand]
    private async Task OpenWatchList(WatchListRowViewModel? row)
    {
        if (row is null)
            return;

        SetOptionsRow(null);

        await ExecuteAsync(async () =>
        {
            await MoviesViewModel.OpenAsync(row);
            IsViewingMovies = true;
            Title = row.Name;
        });
    }

    [RelayCommand]
    private void BackToLists()
    {
        IsViewingMovies = false;
        Title = "WatchLists";
        MoviesViewModel.Clear();
    }

    [RelayCommand]
    private void ToggleWatchListOptions(WatchListRowViewModel? row)
    {
        if (row is null)
            return;

        SetOptionsRow(_openOptionsRow == row ? null : row);
    }

    [RelayCommand]
    private void StartEditWatchList(WatchListRowViewModel? row)
    {
        if (row is null || !row.CanEdit)
            return;

        SetOptionsRow(null);
        NameForm.BeginRename(row);
    }

    [RelayCommand]
    private async Task ConfirmNameForm()
    {
        var name = NameForm.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return;

        if (NameForm.IsCreateMode)
        {
            await ExecuteAsync(async () =>
            {
                await watchListService.CreateWatchListAsync(name);
                NameForm.Close();
                await ReloadWatchListsAsync();
            });

            return;
        }

        if (NameForm.IsRenameMode && NameForm.TargetRow is not null)
        {
            NameForm.TargetRow.RenameLocally(name);
            NameForm.Close();

            await alerts.ShowToastAsync("Renamed locally. Backend update is not implemented yet.");
        }
    }

    [RelayCommand]
    private void CancelNameForm()
    {
        NameForm.Close();
    }

    [RelayCommand]
    private async Task DeleteWatchList(WatchListRowViewModel? row)
    {
        if (row is null || !row.CanDelete)
            return;

        SetOptionsRow(null);

        await ExecuteAsync(async () =>
        {
            await watchListService.DeleteWatchListAsync(row.Item);
            Lists.Remove(row);

            OnPropertyChanged(nameof(HasWatchLists));
            await alerts.ShowToastAsync("List deleted.");
        });
    }

    private async Task ReloadWatchListsAsync()
    {
        Lists.Clear();

        foreach (var list in await watchListService.GetWatchListsAsync())
            Lists.Add(new WatchListRowViewModel(list));

        OnPropertyChanged(nameof(HasWatchLists));
    }

    private void SetOptionsRow(WatchListRowViewModel? row)
    {
        if (_openOptionsRow is not null)
            _openOptionsRow.IsOptionsOpen = false;

        _openOptionsRow = row;

        if (_openOptionsRow is not null)
            _openOptionsRow.IsOptionsOpen = true;
    }
}
