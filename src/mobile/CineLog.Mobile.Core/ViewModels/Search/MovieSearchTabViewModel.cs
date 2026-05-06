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
    private readonly IMovieSearchService searchService;
    private readonly IMovieDetailNavigationContext movieDetailNav;
    private readonly INavigationService navigation;

    private string currentQuery = string.Empty;
    private int page;

    [ObservableProperty] private bool hasQuery;
    [ObservableProperty] private bool showSkeleton;
    [ObservableProperty] private bool showNoResults;
    [ObservableProperty] private bool isLoadingMore;
    [ObservableProperty] private bool canLoadMore;

    public ObservableCollection<MovieItem> Movies { get; } = [];

    public bool ShowResults => HasQuery && Movies.Count > 0;
    public bool ShowEmptyState => !HasQuery;

    public MovieSearchTabViewModel(
        IMovieSearchService searchService,
        IMovieDetailNavigationContext movieDetailNav,
        INavigationService navigation,
        IAlertService alerts) : base(alerts)
    {
        this.searchService = searchService;
        this.movieDetailNav = movieDetailNav;
        this.navigation = navigation;
    }

    public async Task SearchAsync(string query, CancellationToken ct = default)
    {
        HasQuery = !string.IsNullOrWhiteSpace(query);
        ShowNoResults = false;
        CanLoadMore = false;
        Movies.Clear();

        if (!HasQuery)
        {
            RefreshVisibility();
            return;
        }

        currentQuery = query.Trim();
        page = 1;
        ShowSkeleton = true;

        try
        {
            var result = await searchService.SearchMoviesAsync(currentQuery, page, ct);
            AddMovies(result.Items);
            CanLoadMore = result.HasMore;
            ShowNoResults = Movies.Count == 0;
        }
        finally
        {
            if (!ct.IsCancellationRequested)
                ShowSkeleton = false;

            RefreshVisibility();
        }
    }

    [RelayCommand]
    private async Task LoadMore()
    {
        if (IsLoadingMore || !CanLoadMore || string.IsNullOrWhiteSpace(currentQuery))
            return;

        IsLoadingMore = true;

        try
        {
            var result = await searchService.SearchMoviesAsync(currentQuery, ++page);
            AddMovies(result.Items);
            CanLoadMore = result.HasMore;
            RefreshVisibility();
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    [RelayCommand]
    private Task GoToMovie(MovieItem movie)
    {
        movieDetailNav.MovieId = movie.Id;
        return navigation.NavigateToAsync(Routes.MovieDetail);
    }

    private void AddMovies(IEnumerable<MovieItem> movies)
    {
        var existingIds = Movies.Select(x => x.Id).ToHashSet();

        foreach (var movie in movies.Where(x => existingIds.Add(x.Id)))
            Movies.Add(movie);
    }

    private void RefreshVisibility()
    {
        OnPropertyChanged(nameof(ShowResults));
        OnPropertyChanged(nameof(ShowEmptyState));
    }
}
