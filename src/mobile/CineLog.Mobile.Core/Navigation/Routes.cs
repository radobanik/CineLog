namespace CineLog.Mobile.Core.Navigation;

public static class Routes
{
    public const string Login = nameof(Login);
    public const string Register = nameof(Register);

    public const string MainTabs = nameof(MainTabs);

    public const string Dashboard = nameof(Dashboard); // Home tab for now
    public const string Search = nameof(Search);
    public const string Log = nameof(Log);
    public const string Lists = nameof(Lists);
    public const string Profile = nameof(Profile);

    public const string AuthenticatedRoot = MainTabs + "/" + Dashboard;
}
