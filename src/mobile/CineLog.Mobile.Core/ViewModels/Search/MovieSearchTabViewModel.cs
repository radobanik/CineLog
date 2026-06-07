using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Search;

public partial class MovieSearchTabViewModel : BaseViewModel
{
    private const int HomeMoviePageSize = 24;
    private const int MaxHomeMovieLoads = 4;

    private readonly IMovieSearchService searchService;
    private readonly IHomeService homeService;
    private readonly IMovieNavigationContext movieNav;
    private readonly IMovieDetailNavigationContext movieDetailNav;
    private readonly INavigationService navigation;

    private string currentQuery = string.Empty;
    private int searchPage;
    private int homeMovieCount = HomeMoviePageSize;
    private int homeLoadCount;
    private bool searchHasMore;
    private bool homeHasMore = true;
    private bool hasLoadedHome;

    [ObservableProperty] private bool hasQuery;
    [ObservableProperty] private bool showSkeleton;
    [ObservableProperty] private bool showNoResults;
    [ObservableProperty] private bool showHomeSkeleton;

    public ObservableCollection<MovieItem> Movies { get; } = [];
    public ObservableCollection<MovieItem> HomeMovies { get; } = [];

    public bool ShowResults => HasQuery && Movies.Count > 0;
    public bool ShowHome => !HasQuery;
    public bool CanLoadMore => HasQuery ? searchHasMore : homeHasMore;

    public MovieSearchTabViewModel(
        IMovieSearchService searchService,
        IHomeService homeService,
        IMovieNavigationContext movieNav,
        IMovieDetailNavigationContext movieDetailNav,
        INavigationService navigation,
        IAlertService alerts) : base(alerts)
    {
        this.searchService = searchService;
        this.homeService = homeService;
        this.movieNav = movieNav;
        this.movieDetailNav = movieDetailNav;
        this.navigation = navigation;
    }

    public async Task SearchAsync(string query, CancellationToken ct = default)
    {
        HasQuery = !string.IsNullOrWhiteSpace(query);
        ShowNoResults = false;
        Movies.Clear();

        if (!HasQuery)
        {
            currentQuery = string.Empty;
            searchHasMore = false;
            await LoadHomeMoviesAsync(ct);
            RefreshVisibility();
            return;
        }

        currentQuery = query.Trim();
        searchPage = 1;
        searchHasMore = false;
        ShowSkeleton = true;

        try
        {
            var result = await searchService.SearchMoviesAsync(currentQuery, searchPage, ct);
            AddMovies(Movies, result.Items);
            searchHasMore = result.HasMore;
            ShowNoResults = Movies.Count == 0;
        }
        finally
        {
            if (!ct.IsCancellationRequested)
                ShowSkeleton = false;

            RefreshVisibility();
        }
    }

    private async Task LoadHomeMoviesAsync(CancellationToken ct = default)
    {
        if (hasLoadedHome && HomeMovies.Count > 0)
            return;

        HomeMovies.Clear();
        homeMovieCount = HomeMoviePageSize;
        homeLoadCount = 1;
        homeHasMore = true;
        ShowHomeSkeleton = true;

        try
        {
            var movies = await homeService.GetNewReleaseMoviesAsync(homeMovieCount, ct);
            AddMovies(HomeMovies, movies);
            hasLoadedHome = true;
        }
        finally
        {
            if (!ct.IsCancellationRequested)
                ShowHomeSkeleton = false;

            RefreshVisibility();
        }
    }

    [RelayCommand]
    private async Task LoadMore()
    {
        if (HasQuery)
            await LoadMoreSearchResultsAsync();
        else
            await LoadMoreHomeMoviesAsync();
    }

    [RelayCommand]
    private Task GoToMovie(MovieItem movie)
    {
        movieDetailNav.MovieId = movie.Id;
        return navigation.NavigateToAsync(Routes.MovieDetail);
    }

    [RelayCommand]
    private Task GoToCategory(MovieCategory category)
    {
        movieNav.Category = category;
        return navigation.NavigateToAsync(Routes.MoviesCategory);
    }

    private async Task LoadMoreSearchResultsAsync()
    {
        if (IsBusy || !searchHasMore || string.IsNullOrWhiteSpace(currentQuery))
            return;

        IsBusy = true;

        try
        {
            var result = await searchService.SearchMoviesAsync(currentQuery, ++searchPage);
            AddMovies(Movies, result.Items);
            searchHasMore = result.HasMore;
            RefreshVisibility();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadMoreHomeMoviesAsync()
    {
        if (IsBusy || !homeHasMore)
            return;

        IsBusy = true;

        try
        {
            var previousCount = HomeMovies.Count;
            homeMovieCount += HomeMoviePageSize;
            homeLoadCount++;

            var movies = await homeService.GetNewReleaseMoviesAsync(homeMovieCount);
            AddMovies(HomeMovies, movies);

            if (HomeMovies.Count == previousCount || homeLoadCount >= MaxHomeMovieLoads)
                homeHasMore = false;

            RefreshVisibility();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static void AddMovies(ObservableCollection<MovieItem> target, IEnumerable<MovieItem> movies)
    {
        var existingIds = target.Select(x => x.Id).ToHashSet();

        foreach (var movie in movies.Where(x => existingIds.Add(x.Id)))
            target.Add(movie);
    }

    private void RefreshVisibility()
    {
        OnPropertyChanged(nameof(ShowResults));
        OnPropertyChanged(nameof(ShowHome));
        OnPropertyChanged(nameof(CanLoadMore));
    }
}
