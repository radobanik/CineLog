using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile;

public partial class App : Application
{
    public App(ISessionService session, AppShell shell)
    {
        InitializeComponent();

        MainPage = new ContentPage { BackgroundColor = (Color)Resources["Background"] };

        _ = InitializeAsync(session, shell);
    }

    private async Task InitializeAsync(ISessionService session, AppShell shell)
    {
        var restored = await session.TryRestoreSessionAsync();

        MainPage = shell;

        var route = restored
            ? $"//{Routes.AuthenticatedRoot}"
            : $"//{Routes.Login}";

        await shell.GoToAsync(route);
    }
}
