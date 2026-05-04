using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.Movies;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Movies;

public partial class MovieDetailViewModel : BaseViewModel
{
    private const int ReviewPageSize = 5;

    private readonly IMovieDetailService _movieDetailService;
    private readonly IMovieDetailNavigationContext _movieDetailNav;
    private readonly INavigationService _navigation;
    private readonly IReviewsClient _reviewsClient;
    private readonly IMoviesClient _moviesClient;
    private readonly IUsersClient _usersClient;

    [ObservableProperty] private MovieDetailInfo? _movie;
    [ObservableProperty] private string _reviewsCountText = string.Empty;
    [ObservableProperty] private bool _hasNoReviews;
    [ObservableProperty] private bool _isLiked;

    public ObservableCollection<CastMemberItem> Cast { get; } = [];
    public ObservableCollection<ReviewPreviewItem> Reviews { get; } = [];

    public MovieDetailViewModel(
        IMovieDetailService movieDetailService,
        IMovieDetailNavigationContext movieDetailNav,
        INavigationService navigation,
        IReviewsClient reviewsClient,
        IMoviesClient moviesClient,
        IUsersClient usersClient,
        IAlertService alerts)
        : base(alerts)
    {
        _movieDetailService = movieDetailService;
        _movieDetailNav = movieDetailNav;
        _navigation = navigation;
        _reviewsClient = reviewsClient;
        _moviesClient = moviesClient;
        _usersClient = usersClient;
    }

    public override Task OnAppearingAsync() => Load();

    [RelayCommand]
    private async Task Load()
    {
        await ExecuteAsync(async () =>
        {
            var movieId = _movieDetailNav.MovieId;

            var detailTask = _movieDetailService.GetMovieDetailAsync(movieId);
            var reviewsTask = _movieDetailService.GetReviewsAsync(movieId, ReviewPageSize);
            var favoritesTask = _usersClient.GetFavoritesAsync();

            await Task.WhenAll(detailTask, reviewsTask, favoritesTask);

            var detail = detailTask.Result;
            Movie = detail;
            Title = detail.Title;

            Cast.Clear();
            foreach (var member in detail.Cast)
                Cast.Add(member);

            var (reviews, totalCount) = reviewsTask.Result;
            Reviews.Clear();
            foreach (var review in reviews)
                Reviews.Add(review);

            ReviewsCountText = BuildReviewsCountText(totalCount);
            HasNoReviews = Reviews.Count == 0;

            IsLiked = favoritesTask.Result.Any(f => f.Id == movieId);
        });
    }

    [RelayCommand]
    private async Task ToggleLikeMovie()
    {
        var wasLiked = IsLiked;
        IsLiked = !wasLiked;

        try
        {
            if (wasLiked)
                await _moviesClient.RemoveFromFavoritesAsync(_movieDetailNav.MovieId);
            else
                await _moviesClient.AddToFavoritesAsync(_movieDetailNav.MovieId);
        }
        catch
        {
            IsLiked = wasLiked;
        }
    }

    [RelayCommand]
    private Task OpenAddToWatchlist() => _navigation.NavigateToAsync(Routes.AddToWatchlist);

    [RelayCommand]
    private async Task ToggleLike(ReviewPreviewItem review)
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

    [RelayCommand]
    private Task GoBack() => _navigation.NavigateBackAsync();

    private static string BuildReviewsCountText(int total)
    {
        if (total == 0) return string.Empty;
        var formatted = total >= 1000 ? $"{total / 1000.0:0.#}k" : total.ToString();
        return $"See all {formatted}";
    }
}
