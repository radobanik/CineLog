using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Dashboard;

public partial class DashboardViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigation;
    private readonly IAlertService _alerts;

    public DashboardViewModel(IAuthService authService, INavigationService navigation, IAlertService alerts)
    {
        _authService = authService;
        _navigation = navigation;
        _alerts = alerts;
        Title = "Dashboard";
    }

    [RelayCommand]
    private async Task Logout()
    {
        await ExecuteAsync(async () =>
        {
            await _authService.LogoutAsync();
            await _navigation.NavigateToRootAsync(Routes.Login);
        });
    }

    [RelayCommand]
    private async Task GoToProfile()
    {
        await _navigation.NavigateToRootAsync(Routes.Profile);
    }

    protected override async Task OnError(Exception ex)
    {
        await _alerts.ShowAlertAsync("Error", ex.Message);
    }
}
