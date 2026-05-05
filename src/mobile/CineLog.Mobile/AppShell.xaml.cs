using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Pages.MainPages;
using CineLog.Mobile.Pages.Movies;


namespace CineLog.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterDetailRoutes();
    }

    private static void RegisterDetailRoutes()
    {
        // Register detail pages here as they are created.

        // Example:
        // Routing.RegisterRoute(Navigation.Routes.MovieDetail,  typeof(MovieDetailPage));
        Routing.RegisterRoute(Routes.MoviesCategory, typeof(MoviesCategoryPage));
        Routing.RegisterRoute(Routes.MovieWatchList, typeof(MovieWatchListPage));
        Routing.RegisterRoute(Routes.MovieDetail, typeof(MovieDetailPage));
        Routing.RegisterRoute(Routes.AddToWatchlist, typeof(AddToWatchlistPage));
        Routing.RegisterRoute(Routes.MovieReviews, typeof(MovieReviewsPage));
        Routing.RegisterRoute(Routes.EditProfile, typeof(EditProfilePage));
        Routing.RegisterRoute(Routes.AddReview, typeof(AddReviewPage));
    }
}
