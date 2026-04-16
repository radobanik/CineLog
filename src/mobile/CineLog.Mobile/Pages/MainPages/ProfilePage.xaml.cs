using CineLog.Mobile.Core.ViewModels.Profile;

namespace CineLog.Mobile.Pages.MainPages;

public partial class ProfilePage : BasePage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
