using CineLog.Mobile.Core.ViewModels.Movies;

namespace CineLog.Mobile.Pages.Movies;

public partial class MovieDetailPage : BasePage
{
    public MovieDetailPage(MovieDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
