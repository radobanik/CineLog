using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Profile;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUsersClient _usersClient;
    private readonly INavigationService _navigationService;
    private readonly IAlertService _alerts;

    private Guid _id;

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

    public ProfileViewModel(IUsersClient usersClient, INavigationService navigationService, IAlertService alerts)
    {
        _usersClient = usersClient;
        _navigationService = navigationService;
        _alerts = alerts;
        Title = "Profile";
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        await ExecuteAsync(async () =>
        {
            var user = await _usersClient.MeGETAsync();
            if (user != null)
            {
                _id = user.Id ?? Guid.Empty;
                Username = user.Username ?? string.Empty;
                Bio = user.Bio ?? string.Empty;
                AvatarUrl = user.AvatarUrl ?? string.Empty;
                FilmsCount = user.FilmsCount ?? 0;
                FollowersCount = user.FollowersCount ?? 0;
                FollowingCount = user.FollowingCount ?? 0;
            }
            var favouriteResponse = await _usersClient.FavoritesAllAsync();
            if (favouriteResponse != null && favouriteResponse.Count > 0)
            {
                FavouriteMovies = (MovieListItemResponse[])favouriteResponse;
            }

            var reviewsResponse = await _usersClient.ReviewsGET3Async(_id, null, null);
            if (reviewsResponse != null && reviewsResponse.Items != null && reviewsResponse.Items.Count > 0)
            {
                Reviews = (ReviewResponse[])reviewsResponse.Items;
            }
        });
    }


    [RelayCommand]
    private async Task ToDashboard()
    {
        await _navigationService.NavigateToRootAsync(Routes.Dashboard);
    }

    protected override async Task OnError(Exception error)
    {
        await _alerts.ShowAlertAsync("Error", error.Message);
    }
}
