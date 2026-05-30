using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Home;

public partial class HomeViewModel : BaseViewModel
{
    private const int RailPageSize = 12;
    private const int LatestReviewsCount = 5;
    private const int MaxAdditionalLoads = 3;

    private readonly IAuthService _authService;
    private readonly IHomeService _homeService;
    private readonly INavigationService _navigation;
    private readonly IMovieNavigationContext _movieNav;
    private readonly IMovieDetailNavigationContext _movieDetailNav;
    private readonly IReviewsNavigationContext _reviewsNav;

    private int _topRatedCount = RailPageSize;
    private int _topRatedLoadCount;

    [ObservableProperty]
    private bool _isLoadingMoreTopRated;

    [ObservableProperty]
    private bool _hasLoadedOnce;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _canLoadMoreTopRated = true;

    private readonly IReviewsClient _reviewsClient;

    public ObservableCollection<MovieItem> TopRatedMovies { get; } = [];
    public ObservableCollection<ReviewListItem> LatestReviews { get; } = [];

    public HomeViewModel(
     IAuthService authService,
     IHomeService homeService,
     INavigationService navigation,
     IMovieNavigationContext movieNav,
     IMovieDetailNavigationContext movieDetailNav,
     IReviewsNavigationContext reviewsNav,
     IReviewsClient reviewsClient,
     IAlertService alerts)
     : base(alerts)
    {
        _authService = authService;
        _homeService = homeService;
        _navigation = navigation;
        _movieNav = movieNav;
        _movieDetailNav = movieDetailNav;
        _reviewsNav = reviewsNav;
        _reviewsClient = reviewsClient;
        Title = "Home";
    }

    public override Task OnAppearingAsync()
    {
        if (HasLoadedOnce && (TopRatedMovies.Count > 0 || LatestReviews.Count > 0))
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
            _topRatedLoadCount = 1;
            CanLoadMoreTopRated = true;

            await ReloadTopRatedAsync();
            await ReloadLatestReviewsAsync();

            HasLoadedOnce = true;
        });
    }

    [RelayCommand]
    public async Task LoadIfNeeded()
    {
        if (HasLoadedOnce && (TopRatedMovies.Count > 0 || LatestReviews.Count > 0))
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

    private async Task ReloadLatestReviewsAsync()
    {
        LatestReviews.Clear();

        var reviews = await _homeService.GetLatestReviewsAsync(LatestReviewsCount);

        foreach (var review in reviews)
            LatestReviews.Add(review);
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
    public Task GoToMovie(MovieItem movie)
    {
        _movieDetailNav.MovieId = movie.Id;
        return _navigation.NavigateToAsync(Routes.MovieDetail);
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
    public Task GoToAllReviews()
    {
        _reviewsNav.Mode = ReviewsMode.All;
        _reviewsNav.EntityId = Guid.Empty;
        _reviewsNav.FocusReviewId = null;

        return _navigation.NavigateToAsync(Routes.MovieReviews);
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

    [RelayCommand]
    private async Task ToggleLatestReviewLike(ReviewListItem review)
    {
        var wasLiked = review.IsLiked;
        review.IsLiked = !wasLiked;
        review.LikesCount += wasLiked ? -1 : 1;

        try
        {
            await _reviewsClient.ToggleLikeAsync(review.Id);
        }
        catch
        {
            review.IsLiked = wasLiked;
            review.LikesCount += wasLiked ? 1 : -1;
        }
    }


    public override async Task HandleErrorAsync(Exception ex)
    {
        HasError = true;
        ErrorMessage = ex.Message;
        await base.HandleErrorAsync(ex);
    }
}
