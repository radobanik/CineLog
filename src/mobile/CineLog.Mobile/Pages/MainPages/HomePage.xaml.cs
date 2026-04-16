using CineLog.Mobile.Core.ViewModels.Home;

namespace CineLog.Mobile.Pages.MainPages;

public partial class HomePage : BasePage
{
    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
