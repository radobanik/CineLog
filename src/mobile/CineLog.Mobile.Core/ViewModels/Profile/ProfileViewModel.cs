using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CineLog.Mobile.Core.ViewModels.Profile;

public partial class ProfileViewModel(
    IUsersClient usersClient,
    IAlertService alerts)
    : BaseViewModel(alerts)
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _bio = string.Empty;

    [ObservableProperty]
    private string _avatarUrl = string.Empty;

    [ObservableProperty]
    private int _filmsCount;

    [ObservableProperty]
    private int _followersCount;

    [ObservableProperty]
    private int _followingCount;

    [ObservableProperty]
    private MovieListItemResponse[] _favouriteMovies = [];

    [ObservableProperty]
    private ReviewResponse[] _reviews = [];

    protected override async Task LoadAsync()
    {
        var userId = await LoadUserAsync();
        await Task.WhenAll(LoadFavouritesAsync(), LoadReviewsAsync(userId));
    }

    private async Task<Guid> LoadUserAsync()
    {
        var user = await usersClient.MeGETAsync();

        Username = user.Username ?? string.Empty;
        Bio = user.Bio ?? string.Empty;
        AvatarUrl = user.AvatarUrl ?? string.Empty;
        FilmsCount = user.FilmsCount ?? 0;
        FollowersCount = user.FollowersCount ?? 0;
        FollowingCount = user.FollowingCount ?? 0;

        return user.Id ?? Guid.Empty;
    }

    private async Task LoadFavouritesAsync()
    {
        var response = await usersClient.FavoritesAllAsync();
        if (response.Count > 0)
            FavouriteMovies = [.. response];
    }

    private async Task LoadReviewsAsync(Guid userId)
    {
        var response = await usersClient.ReviewsGET3Async(userId, null, null);
        if (response?.Items?.Count > 0)
            Reviews = [.. response.Items];
    }
}
