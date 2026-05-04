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

    [ObservableProperty] private MovieDetailInfo? _movie;
    [ObservableProperty] private string _reviewsCountText = string.Empty;
    [ObservableProperty] private bool _hasNoReviews;

    public ObservableCollection<CastMemberItem> Cast { get; } = [];
    public ObservableCollection<ReviewPreviewItem> Reviews { get; } = [];

    public MovieDetailViewModel(
        IMovieDetailService movieDetailService,
        IMovieDetailNavigationContext movieDetailNav,
        INavigationService navigation,
        IReviewsClient reviewsClient,
        IAlertService alerts)
        : base(alerts)
    {
        _movieDetailService = movieDetailService;
        _movieDetailNav = movieDetailNav;
        _navigation = navigation;
        _reviewsClient = reviewsClient;
    }

    public override Task OnAppearingAsync() => Load();

    [RelayCommand]
    private async Task Load()
    {
        await ExecuteAsync(async () =>
        {
            var movieId = _movieDetailNav.MovieId;

            var detail = await _movieDetailService.GetMovieDetailAsync(movieId);
            Movie = detail;
            Title = detail.Title;

            Cast.Clear();
            foreach (var member in detail.Cast)
                Cast.Add(member);

            var (reviews, totalCount) = await _movieDetailService.GetReviewsAsync(movieId, ReviewPageSize);

            Reviews.Clear();
            foreach (var review in reviews)
                Reviews.Add(review);

            ReviewsCountText = BuildReviewsCountText(totalCount);
            HasNoReviews = Reviews.Count == 0;
        });
    }

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
