using CineLog.Mobile.Core.ViewModels.Search;

namespace CineLog.Mobile.Pages.MainPages;

public partial class SearchPage : BasePage
{
    public SearchPage(SearchViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        SearchEntry.Focused += (_, _) => Shell.SetTabBarIsVisible(this, false);
        SearchEntry.Unfocused += (_, _) => Shell.SetTabBarIsVisible(this, true);
    }
}
