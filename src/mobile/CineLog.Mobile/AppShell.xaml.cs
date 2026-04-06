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
        // Routing.RegisterRoute(Navigation.Routes.MovieDetail,  typeof(MovieDetailPage));
        // Routing.RegisterRoute(Navigation.Routes.ReviewDetail, typeof(ReviewDetailPage));
        // Routing.RegisterRoute(Navigation.Routes.PersonDetail, typeof(PersonDetailPage));
    }
}
