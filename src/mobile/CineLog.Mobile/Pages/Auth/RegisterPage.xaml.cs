using CineLog.Mobile.Core.ViewModels.Auth;

namespace CineLog.Mobile.Pages.Auth;

public partial class RegisterPage : BasePage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
