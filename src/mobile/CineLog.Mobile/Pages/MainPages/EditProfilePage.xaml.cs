using CineLog.Mobile.Core.ViewModels.Profile;

namespace CineLog.Mobile.Pages.MainPages;

public partial class EditProfilePage : BasePage
{
    public EditProfilePage(EditProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
