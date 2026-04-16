using CineLog.Mobile.Core.ViewModels.Common;

namespace CineLog.Mobile.Pages.Movies;

public partial class MoviesCategoryPage : BasePage
{
    public MoviesCategoryPage(MoviesCategoryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
