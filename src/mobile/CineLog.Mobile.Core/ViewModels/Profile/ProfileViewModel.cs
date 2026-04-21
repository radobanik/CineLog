using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.Profile;

public partial class ProfileViewModel(IProfileService profileService, IAlertService alerts)
    : BaseViewModel(alerts)
{
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _bio = string.Empty;
    [ObservableProperty] private string _avatarUrl = string.Empty;
    [ObservableProperty] private int _filmsCount;
    [ObservableProperty] private int _followersCount;
    [ObservableProperty] private int _followingCount;

    public ObservableCollection<MovieItem> FavouriteMovies { get; } = [];
    public ObservableCollection<ReviewItem> Reviews { get; } = [];

    protected override async Task LoadAsync()
    {
        Title = "Profile";

        var profile = await profileService.GetProfileAsync();

        Username = profile.Username;
        Bio = profile.Bio;
        AvatarUrl = profile.AvatarUrl;
        FilmsCount = profile.FilmsCount;
        FollowersCount = profile.FollowersCount;
        FollowingCount = profile.FollowingCount;

        var favourites = await profileService.GetFavouriteMoviesAsync();
        var reviews = await profileService.GetReviewsAsync(profile.Id);

        FavouriteMovies.Clear();
        foreach (var movie in favourites) FavouriteMovies.Add(movie);

        Reviews.Clear();
        foreach (var review in reviews) Reviews.Add(review);
    }
}
