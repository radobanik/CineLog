using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Base;

public abstract partial class BaseListViewModel<T> : BaseViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isRefreshing;

    public ObservableCollection<T> Items { get; } = [];

    public bool IsEmpty => !IsBusy && Items.Count == 0;

    [RelayCommand]
    public async Task LoadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var items = await FetchAsync();
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);

            OnPropertyChanged(nameof(IsEmpty));
        });
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadAsync();
        IsRefreshing = false;
    }

    protected abstract Task<IEnumerable<T>> FetchAsync();
}
