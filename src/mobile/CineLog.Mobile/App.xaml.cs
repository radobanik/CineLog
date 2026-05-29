using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile;

public partial class App : Application
{
    public App(ISessionService session, AppShell shell, IFcmService fcmService)
    {
        InitializeComponent();

        MainPage = shell;

        _ = InitializeAsync(session, shell, fcmService);
    }

    private static async Task InitializeAsync(
        ISessionService session,
        AppShell shell,
        IFcmService fcmService)
    {
        try
        {
            var restored = await session.TryRestoreSessionAsync();

            var route = restored
                ? $"//{Routes.AuthenticatedRoot}"
                : $"//{Routes.Login}";

            await shell.GoToAsync(route);

            await RequestNotificationPermissionAsync();

            if (restored)
                _ = fcmService.RegisterTokenAsync();
        }
        catch
        {
            await shell.GoToAsync($"//{Routes.Login}");
        }
    }

    private static async Task RequestNotificationPermissionAsync()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
            await Permissions.RequestAsync<Permissions.PostNotifications>();
    }
}
