using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile;

public partial class App : Application
{
    public App(ISessionService session, AppShell shell, IFcmService fcmService)
    {
        InitializeComponent();

        MainPage = new ContentPage { BackgroundColor = (Color)Resources["Background"] };

        _ = InitializeAsync(session, shell, fcmService);
    }

    private async Task InitializeAsync(ISessionService session, AppShell shell, IFcmService fcmService)
    {
        await RequestNotificationPermissionAsync();

        var restored = await session.TryRestoreSessionAsync();

        MainPage = shell;

        var route = restored
            ? $"//{Routes.AuthenticatedRoot}"
            : $"//{Routes.Login}";

        await shell.GoToAsync(route);

        if (restored)
            _ = fcmService.RegisterTokenAsync();
    }

    private static async Task RequestNotificationPermissionAsync()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
            await Permissions.RequestAsync<Permissions.PostNotifications>();
    }
}
