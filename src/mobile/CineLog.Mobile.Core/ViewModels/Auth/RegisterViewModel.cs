using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Auth;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigation;
    private readonly IAlertService _alerts;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private bool _isTermsAccepted = false;

    [ObservableProperty]
    private bool _isPasswordVisible = false;

    [ObservableProperty]
    private bool _isConfirmPasswordVisible = false;

    public RegisterViewModel(IAuthService authService, INavigationService navigation, IAlertService alerts)
    {
        _authService = authService;
        _navigation = navigation;
        _alerts = alerts;
        Title = "Create Account";
    }

    private bool CanRegister =>
        !string.IsNullOrWhiteSpace(Username) &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        !string.IsNullOrWhiteSpace(ConfirmPassword) &&
        IsTermsAccepted;

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task Register()
    {
        await ExecuteAsync(async () =>
        {
            await _authService.RegisterAsync(Username, Email, Password);
            await _navigation.NavigateToRootAsync("//Dashboard");
        });
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordVisible = !IsPasswordVisible;

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility() => IsConfirmPasswordVisible = !IsConfirmPasswordVisible;

    [RelayCommand]
    private Task GoToLogin() => _navigation.NavigateBackAsync();

    protected override async Task OnError(Exception ex)
    {
        await _alerts.ShowAlertAsync("Registration failed", ex.Message);
    }
}
