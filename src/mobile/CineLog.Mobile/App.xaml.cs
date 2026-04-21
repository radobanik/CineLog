using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile;

public partial class App : Application
{
    public App(ISessionService session, AppShell shell)
    {
        InitializeComponent();
        MainPage = new ContentPage { BackgroundColor = Color.FromArgb("#0B0D10") };
        _ = InitializeAsync(session, shell);
    }

    private async Task InitializeAsync(ISessionService session, AppShell shell)
    {
        var restored = await session.TryRestoreSessionAsync();

        if (!restored)
            shell.CurrentItem = shell.Items.OfType<ShellContent>().First(x => x.Route == "Login");
        else
            shell.CurrentItem = shell.Items.OfType<TabBar>().First();

        MainPage = shell;
    }
}
