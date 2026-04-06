using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Navigation;

public sealed class ShellNavigationService : INavigationService
{
    public Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (parameters is { Count: > 0 })
            return Shell.Current.GoToAsync(route, parameters);

        return Shell.Current.GoToAsync(route);
    }

    public Task NavigateBackAsync() =>
        Shell.Current.GoToAsync("..");

    public Task NavigateToRootAsync(string route) =>
        Shell.Current.GoToAsync($"//{route}");
}
