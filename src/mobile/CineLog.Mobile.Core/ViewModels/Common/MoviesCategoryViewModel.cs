using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Common;

public partial class MoviesCategoryViewModel : BaseViewModel
{
    private const int PageSize = 24;

    private readonly IHomeService homeService;
    private readonly IMovieSearchService movieSearchService;
    private readonly IMovieNavigationContext movieNav;
    private readonly IMovieDetailNavigationContext movieDetailNav;
    private readonly INavigationService navigation;

    private int currentCount = PageSize;
    private MovieCategory category = MovieCategory.TopRated;

    [ObservableProperty] private bool hasMore = true;
    [ObservableProperty] private bool hasError;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isLoadingMore;

    public ObservableCollection<MovieItem> Movies { get; } = [];

    public MoviesCategoryViewModel(
        IHomeService homeService,
        IMovieSearchService movieSearchService,
        IMovieNavigationContext movieNav,
        IMovieDetailNavigationContext movieDetailNav,
        INavigationService navigation,
        IAlertService alerts) : base(alerts)
    {
        this.homeService = homeService;
        this.movieSearchService = movieSearchService;
        this.movieNav = movieNav;
        this.movieDetailNav = movieDetailNav;
        this.navigation = navigation;
    }

    public override Task OnAppearingAsync()
    {
        category = movieNav.Category;
        Title = GetTitle(category);
        return Load();
    }

    protected override Task LoadAsync() => Load();

    [RelayCommand]
    public Task GoToMovie(MovieItem movie)
    {
        movieDetailNav.MovieId = movie.Id;
        return navigation.NavigateToAsync(Routes.MovieDetail);
    }

    [RelayCommand]
    public async Task Load()
    {
        await ExecuteAsync(async () =>
        {
            HasError = false;
            ErrorMessage = string.Empty;
            currentCount = PageSize;
            HasMore = true;

            var movies = await GetMoviesAsync(currentCount);

            Movies.Clear();
            foreach (var movie in movies)
                Movies.Add(movie);
        });
    }

    [RelayCommand]
    public async Task LoadMore()
    {
        if (IsBusy || IsLoadingMore || !HasMore)
            return;

        try
        {
            IsLoadingMore = true;

            var previousCount = Movies.Count;
            currentCount += PageSize;

            var movies = await GetMoviesAsync(currentCount);
            var existingIds = Movies.Select(x => x.Id).ToHashSet();

            foreach (var movie in movies.Where(x => existingIds.Add(x.Id)))
                Movies.Add(movie);

            if (Movies.Count == previousCount)
                HasMore = false;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    private async Task<IReadOnlyList<MovieItem>> GetMoviesAsync(int count)
    {
        if (category == MovieCategory.TopRated)
            return await homeService.GetTopRatedMoviesAsync(count);

        if (category == MovieCategory.NewReleases)
            return await homeService.GetNewReleaseMoviesAsync(count);

        var result = await movieSearchService.SearchMoviesByCategoryAsync(
            category,
            page: 1,
            pageSize: count);

        HasMore = result.HasMore;
        return result.Items;
    }

    private static string GetTitle(MovieCategory category) => category switch
    {
        MovieCategory.TopRated => "Top Rated",
        MovieCategory.NewReleases => "New Releases",
        MovieCategory.Scifi => "Sci-Fi",
        _ => category.ToString()
    };

    public override Task HandleErrorAsync(Exception ex)
    {
        HasError = true;
        ErrorMessage = ex.Message;
        return Task.CompletedTask;
    }
}
