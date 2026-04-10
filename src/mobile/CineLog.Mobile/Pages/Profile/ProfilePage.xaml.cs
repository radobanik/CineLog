using CineLog.Mobile.Core.ViewModels.Profile;

namespace CineLog.Mobile.Pages.Profile;

public partial class ProfilePage : BasePage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
