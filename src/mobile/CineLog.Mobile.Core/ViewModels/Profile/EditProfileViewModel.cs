using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Infrastructure;
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
    IMediaPickerService mediaPicker,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    private PickedPhoto? _pendingAvatar;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Initial))]
    private string _username = string.Empty;

    [ObservableProperty] private string _avatarUrl = string.Empty;

    private string _bio = string.Empty;
    public string Bio
    {
        get => _bio;
        set
        {
            var lines = value.Split('\n');
            var clamped = lines.Length > 4 ? string.Join('\n', lines.Take(4)) : value;
            SetProperty(ref _bio, clamped);
            OnPropertyChanged(nameof(BioLengthText));
        }
    }

    public string BioLengthText => $"{Bio.Length}/100";

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
    private async Task PickAvatar()
    {
        var photo = await mediaPicker.PickPhotoAsync();
        if (photo is null) return;

        _pendingAvatar = photo;
        AvatarUrl = photo.LocalPath;
    }

    [RelayCommand]
    private async Task Save()
    {
        await ExecuteAsync(async () =>
        {
            if (_pendingAvatar is not null)
            {
                await using var stream = await _pendingAvatar.OpenStreamAsync();
                var file = new FileParameter(stream, _pendingAvatar.FileName, _pendingAvatar.ContentType);
                var uploadResponse = await usersClient.UploadAvatarAsync(file);
                AvatarUrl = uploadResponse.AvatarUrl ?? AvatarUrl;
                _pendingAvatar = null;
            }

            await usersClient.UpdateMeAsync(new UpdateProfileCommand { Bio = Bio, AvatarUrl = AvatarUrl });
            await navigation.NavigateBackAsync();
        });
    }

    [RelayCommand]
    private Task GoBack() => navigation.NavigateBackAsync();
}
