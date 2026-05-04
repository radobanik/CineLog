using CineLog.Mobile.Core.ViewModels.Movies;

namespace CineLog.Mobile.Pages.Movies;

public partial class AddToWatchlistPage : BasePage
{
    public AddToWatchlistPage(AddToWatchlistViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
