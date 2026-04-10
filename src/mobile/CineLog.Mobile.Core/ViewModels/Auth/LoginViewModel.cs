using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Auth;

public partial class LoginViewModel(IAuthService authService, INavigationService navigation, IAlertService alerts)
    : BaseViewModel(alerts)
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isPasswordVisible;

    private bool CanLogin => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        await ExecuteAsync(async () =>
        {
            await authService.LoginAsync(Email, Password);
            await navigation.NavigateToRootAsync(Routes.Dashboard);
        });
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordVisible = !IsPasswordVisible;

    [RelayCommand]
    private Task GoToRegister() => navigation.NavigateToRootAsync(Routes.Register);

    protected override string ErrorTitle => "Login failed";
}
