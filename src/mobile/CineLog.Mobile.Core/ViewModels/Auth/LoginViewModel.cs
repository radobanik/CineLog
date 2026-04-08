using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Auth;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigation;
    private readonly IAlertService _alerts;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isPasswordVisible = false;

    public LoginViewModel(IAuthService authService, INavigationService navigation, IAlertService alerts)
    {
        _authService = authService;
        _navigation = navigation;
        _alerts = alerts;
        Title = "Sign In";
    }

    private bool CanLogin => !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        await ExecuteAsync(async () =>
        {
            await _authService.LoginAsync(Email, Password);
            await _navigation.NavigateToRootAsync(Routes.AuthenticatedRoot);
        });
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordVisible = !IsPasswordVisible;

    [RelayCommand]
    private Task GoToRegister() => _navigation.NavigateToRootAsync(Routes.Register);

    protected override async Task OnError(Exception ex)
    {
        await _alerts.ShowAlertAsync("Login failed", ex.Message);
    }
}
