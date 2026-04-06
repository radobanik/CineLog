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
    }
}
