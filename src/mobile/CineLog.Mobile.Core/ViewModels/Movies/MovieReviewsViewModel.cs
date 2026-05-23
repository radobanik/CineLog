using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Review;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Movies;

public partial class MovieReviewsViewModel(
    IMovieDetailService movieDetailService,
    IProfileService profileService,
    IReviewsClient reviewsClient,
    IReviewsNavigationContext reviewsNav,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    private const int PageSize = 10;
    private int _currentPage = 0;
    private int _totalPages = 0;

    [ObservableProperty] private bool _canLoadMore;
    [ObservableProperty] private bool _isLoadingMore;

    public bool IsMovieMode => reviewsNav.Mode == ReviewsMode.Movie;
    public bool IsUserMode => reviewsNav.Mode == ReviewsMode.User;

    public ObservableCollection<ReviewListItem> Reviews { get; } = [];

    protected override async Task LoadAsync()
    {
        Title = "Reviews";
        _currentPage = 0;
        _totalPages = 0;
        Reviews.Clear();
        CanLoadMore = false;

        await LoadFocusedReviewAsync();
        await FetchNextPageAsync();
    }

    [RelayCommand]
    private async Task LoadMore()
    {
        if (IsLoadingMore || IsBusy || !CanLoadMore) return;

        IsLoadingMore = true;
        try
        {
            await FetchNextPageAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    private async Task LoadFocusedReviewAsync()
    {
        if (reviewsNav.FocusReviewId is not { } reviewId)
            return;

        reviewsNav.FocusReviewId = null;

        try
        {
            var review = await reviewsClient.GetByIdAsync(reviewId);
            Reviews.Add(MapReview(review));
        }
        catch
        {
            // Deleted reviews can still have activity rows. In that case the normal list is enough.
        }
    }

    private async Task FetchNextPageAsync()
    {
        IReadOnlyList<ReviewListItem> items;
        int totalPages;

        if (reviewsNav.Mode == ReviewsMode.Movie)
        {
            var (i, _, tp) = await movieDetailService.GetReviewsPageAsync(reviewsNav.EntityId, _currentPage + 1, PageSize);
            items = i;
            totalPages = tp;
        }
        else
        {
            var (i, _, tp) = await profileService.GetReviewsPageAsync(reviewsNav.EntityId, _currentPage + 1, PageSize);
            items = i;
            totalPages = tp;
        }

        foreach (var item in items)
        {
            if (Reviews.All(existing => existing.Id != item.Id))
                Reviews.Add(item);
        }

        _currentPage++;
        _totalPages = totalPages;
        CanLoadMore = _currentPage < _totalPages;
    }

    [RelayCommand]
    private async Task ToggleLike(ReviewListItem review)
    {
        if (!IsMovieMode) return;

        var wasLiked = review.IsLiked;
        review.IsLiked = !wasLiked;
        review.LikesCount += wasLiked ? -1 : 1;

        try
        {
            await reviewsClient.ToggleLikeAsync(review.Id);
        }
        catch
        {
            review.IsLiked = wasLiked;
            review.LikesCount += wasLiked ? 1 : -1;
        }
    }

    [RelayCommand]
    private Task GoBack() => navigation.NavigateBackAsync();

    private static ReviewListItem MapReview(ReviewResponse review) => new()
    {
        Id = review.Id ?? Guid.Empty,
        Username = review.Username,
        AvatarUrl = review.AvatarUrl,
        MovieTitle = review.MovieTitle ?? string.Empty,
        Rating = review.Rating,
        ReviewText = review.ReviewText,
        CreatedAt = review.CreatedAt,
        LikesCount = review.LikesCount ?? 0,
        IsLiked = review.IsLiked ?? false
    };
}
