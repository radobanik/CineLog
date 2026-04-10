using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Dashboard;

public partial class DashboardViewModel(IAuthService authService, INavigationService navigation, IAlertService alerts)
    : BaseViewModel(alerts)
{
    [RelayCommand]
    private async Task Logout()
    {
        await ExecuteAsync(async () =>
        {
            await authService.LogoutAsync();
            await navigation.NavigateToRootAsync(Routes.Login);
        });
    }

    [RelayCommand]
    private async Task GoToProfile()
    {
        await navigation.NavigateToRootAsync(Routes.Profile);
    }

}
