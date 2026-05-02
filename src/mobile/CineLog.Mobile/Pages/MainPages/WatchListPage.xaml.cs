using CineLog.Mobile.Core.ViewModels.WatchList;
using System.ComponentModel;

namespace CineLog.Mobile.Pages.MainPages;

public partial class WatchListsPage : BasePage
{
    private readonly WatchListViewModel _vm;

    public WatchListsPage(WatchListViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = vm;

        _vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(WatchListViewModel.IsBusy) || _vm.IsBusy)
            return;

        ScrollToTopAfterLayout();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ScrollToTopAfterLayout();
    }

    private void ScrollToTopAfterLayout()
    {
        Dispatcher.DispatchDelayed(
            TimeSpan.FromMilliseconds(250),
            () => WatchListsCollection.ScrollTo(0, position: ScrollToPosition.Start, animate: false));
    }
}
