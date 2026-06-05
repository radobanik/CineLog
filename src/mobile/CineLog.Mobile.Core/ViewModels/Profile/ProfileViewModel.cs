using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
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
    ISessionService session,
    IFollowService followService,
    IReviewsClient reviewsClient,
    IMovieDetailNavigationContext movieDetailNav,
    IReviewsNavigationContext reviewsNav,
    IEditReviewNavigationContext editReviewNav,
    INavigationService navigation,
    IAlertService alerts)
    : BaseViewModel(alerts)
{
    private const string SignOutIcon = "\uf2f5";

    private Guid _userId;
    private Guid? _requestedUserId;

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

    [ObservableProperty] private int _filmsCount;
    [ObservableProperty] private int _followersCount;
    [ObservableProperty] private int _followingCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSelfActions))]
    [NotifyPropertyChangedFor(nameof(ShowFollowButton))]
    [NotifyPropertyChangedFor(nameof(ProfileNavRightIcon))]
    private bool _isOwnProfile = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FollowButtonText))]
    private bool _isFollowing;

    public bool ShowExpandButton => !IsBioExpanded && BioIsLong;
    public bool ShowCollapseButton => IsBioExpanded && BioIsLong;
    public bool ShowSelfActions => IsOwnProfile;
    public bool ShowFollowButton => !IsOwnProfile;
    public string FollowButtonText => IsFollowing ? "Unfollow" : "Follow";
    public string ProfileNavRightIcon => IsOwnProfile ? SignOutIcon : string.Empty;

    private bool BioIsLong => (Bio.Count('\n') >= 2 && Bio.Length < 50) || Bio.Length > 50;

    public ObservableCollection<MovieItem> FavouriteMovies { get; } = [];
    public ObservableCollection<ReviewListItem> Reviews { get; } = [];

    public void ShowCurrentUser() => _requestedUserId = null;

    public void ShowUser(Guid userId) =>
        _requestedUserId = userId == Guid.Empty ? null : userId;

    [RelayCommand]
    public Task GoToMovie(MovieItem movie)
    {
        movieDetailNav.MovieId = movie.Id;
        return navigation.NavigateToAsync(Routes.MovieDetail);
    }

    [RelayCommand]
    private Task GoToMovieFromReview(ReviewListItem review)
    {
        movieDetailNav.MovieId = review.MovieId;
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
    private Task GoToEditReview(ReviewListItem review)
    {
        if (!IsOwnProfile)
            return Task.CompletedTask;

        editReviewNav.ReviewId = review.Id;
        editReviewNav.MovieTitle = review.MovieTitle;
        editReviewNav.Rating = review.Rating ?? 0.0;
        editReviewNav.ReviewText = review.ReviewText;
        return navigation.NavigateToAsync(Routes.AddReview);
    }

    [RelayCommand]
    private async Task ToggleFollow()
    {
        if (IsOwnProfile || _userId == Guid.Empty)
            return;

        var wasFollowing = IsFollowing;
        IsFollowing = !wasFollowing;
        ApplyFollowerDelta(IsFollowing);

        try
        {
            if (wasFollowing)
                await followService.UnfollowAsync(_userId);
            else
                await followService.FollowAsync(_userId);
        }
        catch (Exception ex)
        {
            IsFollowing = wasFollowing;
            ApplyFollowerDelta(wasFollowing);
            await HandleErrorAsync(ex);
        }
    }

    [RelayCommand]
    private async Task ToggleLike(ReviewListItem review)
    {
        if (IsOwnProfile)
            return;

        var wasLiked = review.IsLiked;
        review.IsLiked = !wasLiked;
        review.LikesCount += wasLiked ? -1 : 1;

        try
        {
            await reviewsClient.ToggleLikeAsync(review.Id);
        }
        catch (Exception ex)
        {
            review.IsLiked = wasLiked;
            review.LikesCount += wasLiked ? 1 : -1;
            await HandleErrorAsync(ex);
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        if (!await alerts.ShowConfirmAsync("Sign out", "Are you sure you want to sign out?"))
            return;

        await authService.LogoutAsync();
        await navigation.NavigateToRootAsync(Routes.Login);
    }

    protected override async Task LoadAsync()
    {
        var requestedUserId = _requestedUserId;
        var loadCurrentUser = requestedUserId is null || requestedUserId == session.UserId;

        var profile = await profileService.GetProfileAsync(loadCurrentUser ? null : requestedUserId);

        _userId = profile.Id;
        IsOwnProfile = profile.Id == session.UserId;
        IsFollowing = !IsOwnProfile && profile.IsFollowing;
        Title = IsOwnProfile ? "Profile" : profile.Username;

        Username = profile.Username;
        Bio = profile.Bio;
        IsBioExpanded = false;
        AvatarUrl = profile.AvatarUrl;
        FilmsCount = profile.FilmsCount;
        FollowersCount = profile.FollowersCount;
        FollowingCount = profile.FollowingCount;

        FavouriteMovies.Clear();
        if (IsOwnProfile)
        {
            var favourites = await profileService.GetFavouriteMoviesAsync();
            foreach (var movie in favourites)
                FavouriteMovies.Add(movie);
        }

        var reviews = await profileService.GetReviewsAsync(profile.Id);

        Reviews.Clear();
        foreach (var review in reviews)
            Reviews.Add(review);
    }

    private void ApplyFollowerDelta(bool isNowFollowing)
    {
        FollowersCount = Math.Max(0, FollowersCount + (isNowFollowing ? 1 : -1));
    }
}
