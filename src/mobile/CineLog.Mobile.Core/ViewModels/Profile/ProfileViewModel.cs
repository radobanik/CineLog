using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Profile;

public partial class ProfileViewModel(
    IProfileService profileService,
    IAuthService authService,
    IMovieDetailNavigationContext movieDetailNav,
    IReviewsNavigationContext reviewsNav,
    INavigationService navigation,
    IAlertService alerts)
    : BaseViewModel(alerts)
{
    private Guid _userId;

    [ObservableProperty] private string _username = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowExpandButton))]
    [NotifyPropertyChangedFor(nameof(ShowCollapseButton))]
    private string _bio = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowExpandButton))]
    [NotifyPropertyChangedFor(nameof(ShowCollapseButton))]
    private bool _isBioExpanded;

    [ObservableProperty] private string _avatarUrl = string.Empty;

    public bool ShowExpandButton => !IsBioExpanded && BioIsLong;
    public bool ShowCollapseButton => IsBioExpanded && BioIsLong;

    private bool BioIsLong => Bio.Count('\n') >= 1 || Bio.Length > 50;
    [ObservableProperty] private int _filmsCount;
    [ObservableProperty] private int _followersCount;
    [ObservableProperty] private int _followingCount;

    public ObservableCollection<MovieItem> FavouriteMovies { get; } = [];
    public ObservableCollection<ReviewItem> Reviews { get; } = [];

    [RelayCommand]
    public Task GoToMovie(MovieItem movie)
    {
        movieDetailNav.MovieId = movie.Id;
        return navigation.NavigateToAsync(Routes.MovieDetail);
    }

    [RelayCommand]
    private void ToggleBio() => IsBioExpanded = !IsBioExpanded;

    [RelayCommand]
    private Task OpenEditProfile() => navigation.NavigateToAsync(Routes.EditProfile);

    [RelayCommand]
    private Task OpenAllReviews()
    {
        reviewsNav.Mode = ReviewsMode.User;
        reviewsNav.EntityId = _userId;
        return navigation.NavigateToAsync(Routes.MovieReviews);
    }

    [RelayCommand]
    private async Task Logout()
    {
        await authService.LogoutAsync();
        await navigation.NavigateToRootAsync(Routes.Login);
    }

    protected override async Task LoadAsync()
    {
        Title = "Profile";

        var profile = await profileService.GetProfileAsync();

        _userId = profile.Id;
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
