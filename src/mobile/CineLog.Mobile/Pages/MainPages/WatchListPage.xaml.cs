using CineLog.Mobile.Core.ViewModels.WatchList;

namespace CineLog.Mobile.Pages.MainPages;

public partial class WatchListsPage : BasePage
{
    private readonly WatchListViewModel _vm;

    public WatchListsPage(WatchListViewModel vm)
    {
        InitializeComponent();

        _vm = vm;
        BindingContext = vm;

        _vm.ScrollToBottomRequested += OnScrollToBottomRequested;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ScrollToTopAfterLayout();
    }

    private void OnScrollToBottomRequested(object? sender, EventArgs e)
    {
        ScrollToBottomAfterLayout();
    }

    private void ScrollToTopAfterLayout()
    {
        Dispatcher.DispatchDelayed(
            TimeSpan.FromMilliseconds(250),
            () => WatchListsCollection.ScrollTo(0, position: ScrollToPosition.Start, animate: false));
    }

    private void ScrollToBottomAfterLayout()
    {
        Dispatcher.DispatchDelayed(
            TimeSpan.FromMilliseconds(250),
            () =>
            {
                if (_vm.Lists.Count == 0)
                    return;

                WatchListsCollection.ScrollTo(
                    _vm.Lists.Count - 1,
                    position: ScrollToPosition.End,
                    animate: true);
            });
    }
}
