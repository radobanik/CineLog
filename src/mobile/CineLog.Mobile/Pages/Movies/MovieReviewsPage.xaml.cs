using CineLog.Mobile.Core.ViewModels.Movies;

namespace CineLog.Mobile.Pages.Movies;

public partial class MovieReviewsPage : BasePage
{
    public MovieReviewsPage(MovieReviewsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
