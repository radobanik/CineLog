using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Base;

public abstract partial class BaseListViewModel<T>(IAlertService alerts) : BaseViewModel(alerts)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isRefreshing;

    public ObservableCollection<T> Items { get; } = [];

    public bool IsEmpty => !IsBusy && Items.Count == 0;

    protected override async Task LoadAsync()
    {
        var items = await FetchAsync();
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);

        OnPropertyChanged(nameof(IsEmpty));
    }

    [RelayCommand]
    protected async Task RefreshAsync()
    {
        IsRefreshing = true;
        await ExecuteAsync(LoadAsync);
        IsRefreshing = false;
    }

    protected abstract Task<IEnumerable<T>> FetchAsync();
}
