using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile;

public partial class App : Application
{
    private readonly ISessionService _session;

    public App(ISessionService session, AppShell shell)
    {
        InitializeComponent();
        _session = session;
        MainPage = shell;
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Restore persisted session on launch
        var restored = await _session.TryRestoreSessionAsync();

        if (!restored)
            await Shell.Current.GoToAsync($"//{Navigation.Routes.Login}");
        else
            await Shell.Current.GoToAsync($"//{Navigation.Routes.Dashboard}");
    }
}
