using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Home;

public partial class HomeViewModel : BaseViewModel
{
    private const int BrowsePageSize = 24;

    private readonly IAuthService _authService;
    private readonly IHomeService _homeService;
    private readonly INavigationService _navigation;
    private readonly IAlertService _alerts;

    private int _nextBrowsePage = 1;

    [ObservableProperty]
    private bool _hasLoadedOnce;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<HomeMovieItem> TopRatedMovies { get; } = [];
    public ObservableCollection<HomeMovieItem> NewReleaseMovies { get; } = [];

    public HomeViewModel(IAuthService authService, IHomeService homeService, INavigationService navigation, IAlertService alerts)
    {
        _authService = authService;
        _homeService = homeService;
        _navigation = navigation;
        _alerts = alerts;
        Title = "Home";
    }

    [RelayCommand]
    public async Task Load()
    {
        await ExecuteAsync(async () =>
        {
            HasError = false;
            ErrorMessage = string.Empty;

            TopRatedMovies.Clear();
            NewReleaseMovies.Clear();

            try
            {
                var topRated = await _homeService.GetTopRatedMoviesAsync();
                foreach (var movie in topRated)
                    TopRatedMovies.Add(movie);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Top Rated failed: {ex.Message}";
            }

            try
            {
                var newReleases = await _homeService.GetNewReleaseMoviesAsync();
                foreach (var movie in newReleases)
                    NewReleaseMovies.Add(movie);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = string.IsNullOrWhiteSpace(ErrorMessage)
                    ? $"New Releases failed: {ex.Message}"
                    : $"{ErrorMessage}\nNew Releases failed: {ex.Message}";
            }

            HasLoadedOnce = true;
        });
    }

    [RelayCommand]
    public async Task LoadIfNeeded()
    {
        if (HasLoadedOnce && (TopRatedMovies.Count > 0 || NewReleaseMovies.Count > 0))
            return;

        await Load();
    }

    [RelayCommand]
    private async Task Logout()
    {
        await ExecuteAsync(async () =>
        {
            await _authService.LogoutAsync();
            await _navigation.NavigateToRootAsync(Routes.Login);
        });
    }

    protected override async Task OnError(Exception ex)
    {
        await _alerts.ShowAlertAsync("Error", ex.Message);
    }
}
