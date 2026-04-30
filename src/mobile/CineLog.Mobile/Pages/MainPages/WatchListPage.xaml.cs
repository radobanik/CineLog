using CineLog.Mobile.Core.ViewModels.WatchList;

namespace CineLog.Mobile.Pages.MainPages;

public partial class WatchListsPage : BasePage
{
    public WatchListsPage(WatchListViewModel wvm)
    {
        InitializeComponent();
        BindingContext = wvm;
    }
}
