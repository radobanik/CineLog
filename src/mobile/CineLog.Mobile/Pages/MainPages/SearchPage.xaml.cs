using CineLog.Mobile.Core.ViewModels.Search;

namespace CineLog.Mobile.Pages.MainPages;

public partial class SearchPage : BasePage
{
    public SearchPage(SearchViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        SkeletonGrid.ItemsSource = Enumerable.Range(0, 12).ToList();
    }
}
