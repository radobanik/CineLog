using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.Movies;
using CineLog.Mobile.Core.Models.Review;
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
    private readonly IReviewsNavigationContext _reviewsNav;
    private readonly INavigationService _navigation;
    private readonly IReviewsClient _reviewsClient;
    private readonly IMoviesClient _moviesClient;

    [ObservableProperty] private MovieDetailInfo? _movie;
    [ObservableProperty] private string _reviewsCountText = string.Empty;
    [ObservableProperty] private bool _hasNoReviews;
    [ObservableProperty] private bool _hasNoOverview;
    [ObservableProperty] private bool _hasNoCast;
    [ObservableProperty] private bool _isLiked;

    public ObservableCollection<CastMemberItem> Cast { get; } = [];
    public ObservableCollection<ReviewListItem> Reviews { get; } = [];

    public MovieDetailViewModel(
        IMovieDetailService movieDetailService,
        IMovieDetailNavigationContext movieDetailNav,
        IReviewsNavigationContext reviewsNav,
        INavigationService navigation,
        IReviewsClient reviewsClient,
        IMoviesClient moviesClient,
        IAlertService alerts)
        : base(alerts)
    {
        _movieDetailService = movieDetailService;
        _movieDetailNav = movieDetailNav;
        _reviewsNav = reviewsNav;
        _navigation = navigation;
        _reviewsClient = reviewsClient;
        _moviesClient = moviesClient;
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

            await Task.WhenAll(detailTask, reviewsTask);

            var detail = detailTask.Result;
            Movie = detail;
            Title = detail.Title;
            IsLiked = detail.IsFavorite;
            HasNoOverview = string.IsNullOrWhiteSpace(detail.Overview);

            Cast.Clear();
            foreach (var member in detail.Cast)
                Cast.Add(member);
            HasNoCast = Cast.Count == 0;

            var (reviews, totalCount) = reviewsTask.Result;
            Reviews.Clear();
            foreach (var review in reviews)
                Reviews.Add(review);

            ReviewsCountText = BuildReviewsCountText(totalCount);
            HasNoReviews = Reviews.Count == 0;
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
    private Task OpenReviews()
    {
        _reviewsNav.Mode = ReviewsMode.Movie;
        _reviewsNav.EntityId = _movieDetailNav.MovieId;
        return _navigation.NavigateToAsync(Routes.MovieReviews);
    }

    [RelayCommand]
    private async Task ToggleLike(ReviewListItem review)
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
