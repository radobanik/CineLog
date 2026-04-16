using CineLog.Mobile.Core.ViewModels.Auth;

namespace CineLog.Mobile.Pages.Auth;

public partial class LoginPage : BasePage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
