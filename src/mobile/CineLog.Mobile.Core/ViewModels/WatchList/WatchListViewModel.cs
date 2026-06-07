using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.WatchList;

public partial class WatchListViewModel(
    IWatchListService watchListService,
    IWatchListNavigationContext watchListNavigation,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    private WatchListRowViewModel? _openOptionsRow;

    public bool HasWatchLists => Lists.Count > 0;

    public WatchListNameFormViewModel NameForm { get; } = new();
    public ObservableCollection<WatchListRowViewModel> Lists { get; } = [];
    public event EventHandler? ScrollToBottomRequested;

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
        watchListNavigation.SelectedRow = row;

        await navigation.NavigateToAsync(Routes.MovieWatchList);
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
                ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);
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
    private void CancelNameForm() => NameForm.Close();

    [RelayCommand]
    private async Task DeleteWatchList(WatchListRowViewModel? row)
    {
        if (row is null || !row.CanDelete)
            return;

        SetOptionsRow(null);

        await ExecuteAsync(async () =>
        {
            await watchListService.DeleteWatchListAsync(row.Item);

            if (row.IsFirstCustom)
            {
                var idx = Lists.IndexOf(row);
                var next = Lists.Skip(idx + 1).FirstOrDefault(r => !r.IsDefault);
                if (next is not null)
                    next.IsFirstCustom = true;
            }

            Lists.Remove(row);
            OnPropertyChanged(nameof(HasWatchLists));
            await alerts.ShowToastAsync("List deleted.");
        });
    }

    private async Task ReloadWatchListsAsync()
    {
        Lists.Clear();

        var rows = (await watchListService.GetWatchListsAsync())
            .Select(list => new WatchListRowViewModel(list))
            .ToList();

        var firstCustom = rows.FirstOrDefault(r => !r.IsDefault);
        if (firstCustom is not null)
            firstCustom.IsFirstCustom = true;

        foreach (var row in rows)
            Lists.Add(row);

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
