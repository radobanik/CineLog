namespace CineLog.Mobile.Core.Navigation;

public static class Routes
{
    public const string Login = nameof(Login);
    public const string Register = nameof(Register);

    public const string MainTabs = nameof(MainTabs);

    public const string Home = nameof(Home);
    public const string Search = nameof(Search);
    public const string Log = nameof(Log);
    public const string Lists = nameof(Lists);
    public const string Profile = nameof(Profile);

    public const string MoviesCategory = nameof(MoviesCategory);

    public const string AuthenticatedRoot = MainTabs + "/" + Home;
}
