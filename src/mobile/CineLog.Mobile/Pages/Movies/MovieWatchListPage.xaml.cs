using CineLog.Mobile.Core.ViewModels.WatchList;

namespace CineLog.Mobile.Pages.Movies;

public partial class MovieWatchListPage : BasePage
{
    public MovieWatchListPage(WatchListMoviesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
