using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.ViewModels.Common;

namespace CineLog.Mobile.Pages.Movies;

public partial class MoviesCategoryPage : BasePage, IQueryAttributable
{
    private readonly MoviesCategoryViewModel _vm;

    public MoviesCategoryPage(MoviesCategoryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var category = query.TryGetValue("category", out var raw) && raw is string s && s == "new-releases"
            ? MovieCategory.NewReleases
            : MovieCategory.TopRated;

        _vm.SetCategory(category);
    }


}
