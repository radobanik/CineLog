using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Profile;

public partial class EditProfileViewModel(
    IProfileService profileService,
    IUsersClient usersClient,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Initial))]
    private string _username = string.Empty;

    [ObservableProperty] private string _avatarUrl = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BioLengthText))]
    private string _bio = string.Empty;

    public string BioLengthText => $"{Bio.Length}/200";

    public string Initial => Username.Length > 0 ? Username[0].ToString().ToUpper() : "?";

    protected override async Task LoadAsync()
    {
        Title = "Edit Profile";
        var profile = await profileService.GetProfileAsync();
        Username = profile.Username;
        AvatarUrl = profile.AvatarUrl;
        Bio = profile.Bio;
    }

    [RelayCommand]
    private async Task Save()
    {
        await ExecuteAsync(async () =>
        {
            await usersClient.UpdateMeAsync(new UpdateProfileCommand { Bio = Bio });
            await navigation.NavigateBackAsync();
        });
    }

    [RelayCommand]
    private Task GoBack() => navigation.NavigateBackAsync();
}
