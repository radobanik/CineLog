namespace CineLog.Mobile.Core.Services.Interfaces;

public interface INavigationService
{
    Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);
    Task NavigateBackAsync();
    Task NavigateToRootAsync(string route);
}
