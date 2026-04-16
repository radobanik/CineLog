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
        var category = MovieCategory.TopRated;

        if (query.TryGetValue("category", out var categoryObj) && categoryObj is string rawCategory)
        {
            category = rawCategory switch
            {
                "new-releases" => MovieCategory.NewReleases,
                _ => MovieCategory.TopRated
            };
        }

        _vm.SetCategory(category);
    }

    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Shell.Current?.Navigation?.NavigationStack?.Count > 1)
                await Shell.Current.GoToAsync("..");
        });

        return true;
    }
}
