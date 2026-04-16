using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Home;

public partial class HomeViewModel : BaseViewModel
{
    private const int RailPageSize = 12;
    private const int MaxAdditionalLoads = 3;

    private readonly IAuthService _authService;
    private readonly IHomeService _homeService;
    private readonly INavigationService _navigation;
    private readonly IMovieNavigationContext _movieNav;

    private int _topRatedCount = RailPageSize;
    private int _newReleaseCount = RailPageSize;
    private int _topRatedLoadCount;
    private int _newReleaseLoadCount;

    [ObservableProperty]
    private bool _isLoadingMoreTopRated;

    [ObservableProperty]
    private bool _isLoadingMoreNewReleases;

    [ObservableProperty]
    private bool _hasLoadedOnce;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _canLoadMoreTopRated = true;

    [ObservableProperty]
    private bool _canLoadMoreNewReleases = true;

    public ObservableCollection<MovieItem> TopRatedMovies { get; } = [];
    public ObservableCollection<MovieItem> NewReleaseMovies { get; } = [];

    public HomeViewModel(IAuthService authService, IHomeService homeService, INavigationService navigation, IMovieNavigationContext movieNav, IAlertService alerts)
        : base(alerts)
    {
        _authService = authService;
        _homeService = homeService;
        _navigation = navigation;
        _movieNav = movieNav;
        Title = "Home";
    }

    public override Task OnAppearingAsync()
    {
        if (HasLoadedOnce && (TopRatedMovies.Count > 0 || NewReleaseMovies.Count > 0))
            return Task.CompletedTask;

        return Load();
    }

    [RelayCommand]
    public async Task Load()
    {
        await ExecuteAsync(async () =>
        {
            HasError = false;
            ErrorMessage = string.Empty;

            _topRatedCount = RailPageSize;
            _newReleaseCount = RailPageSize;
            _topRatedLoadCount = 1;
            _newReleaseLoadCount = 1;

            await ReloadTopRatedAsync();
            await ReloadNewReleasesAsync();

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

    private async Task ReloadTopRatedAsync(bool appendOnly = false)
    {
        var movies = await _homeService.GetTopRatedMoviesAsync(_topRatedCount);

        if (appendOnly)
            AppendOnlyNewMovies(TopRatedMovies, movies);
        else
            ReplaceMovies(TopRatedMovies, movies);
    }

    private async Task ReloadNewReleasesAsync(bool appendOnly = false)
    {
        var movies = await _homeService.GetNewReleaseMoviesAsync(_newReleaseCount);

        if (appendOnly)
            AppendOnlyNewMovies(NewReleaseMovies, movies);
        else
            ReplaceMovies(NewReleaseMovies, movies);
    }

    private static void ReplaceMovies(
    ObservableCollection<MovieItem> target,
    IEnumerable<MovieItem> movies)
    {
        target.Clear();
        foreach (var movie in movies)
            target.Add(movie);
    }

    private static void AppendOnlyNewMovies(
    ObservableCollection<MovieItem> target,
    IEnumerable<MovieItem> movies)
    {
        var existingIds = target.Select(x => x.Id).ToHashSet();

        foreach (var movie in movies.Where(x => !existingIds.Contains(x.Id)))
            target.Add(movie);
    }
    [RelayCommand]
    public async Task LoadMoreTopRated()
    {
        if (IsBusy || IsLoadingMoreTopRated || !CanLoadMoreTopRated)
            return;

        try
        {
            IsLoadingMoreTopRated = true;
            _topRatedCount += RailPageSize;
            _topRatedLoadCount++;

            await ReloadTopRatedAsync(appendOnly: true);

            if (_topRatedLoadCount >= MaxAdditionalLoads)
                CanLoadMoreTopRated = false;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoadingMoreTopRated = false;
        }
    }

    [RelayCommand]
    public async Task LoadMoreNewReleases()
    {
        if (IsBusy || IsLoadingMoreNewReleases || !CanLoadMoreNewReleases)
            return;

        try
        {
            IsLoadingMoreNewReleases = true;
            _newReleaseCount += RailPageSize;
            _newReleaseLoadCount++;

            await ReloadNewReleasesAsync(appendOnly: true);

            if (_newReleaseLoadCount >= MaxAdditionalLoads)
                CanLoadMoreNewReleases = false;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoadingMoreNewReleases = false;
        }
    }

    [RelayCommand]
    public Task GoToTopRated()
    {
        _movieNav.Category = MovieCategory.TopRated;
        return _navigation.NavigateToAsync(Routes.MoviesCategory);
    }

    [RelayCommand]
    public Task GoToNewReleases()
    {
        _movieNav.Category = MovieCategory.NewReleases;
        return _navigation.NavigateToAsync(Routes.MoviesCategory);
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

    public override async Task HandleErrorAsync(Exception ex)
    {
        HasError = true;
        ErrorMessage = ex.Message;
        await base.HandleErrorAsync(ex);
    }
}
