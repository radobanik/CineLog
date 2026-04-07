using CineLog.Mobile.Core.ViewModels.Profile;

namespace CineLog.Mobile.Pages.Profile;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
        {
            vm.LoadProfileCommand.Execute(null);
        }
    }
}
