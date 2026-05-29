using CineLog.Mobile.Core.ViewModels.Movies;

namespace CineLog.Mobile.Pages.Movies;

public partial class AddReviewPage : BasePage
{
    public AddReviewPage(AddReviewViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
