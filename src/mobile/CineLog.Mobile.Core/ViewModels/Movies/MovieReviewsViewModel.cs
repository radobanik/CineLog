using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.Core.Models.Movies;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Movies;

public partial class MovieReviewsViewModel(
    IMovieDetailService movieDetailService,
    IReviewsClient reviewsClient,
    IMovieDetailNavigationContext movieDetailNav,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    private const int PageSize = 10;
    private int _currentPage = 0;
    private int _totalPages = 0;

    [ObservableProperty] private bool _canLoadMore;
    [ObservableProperty] private bool _isLoadingMore;

    public ObservableCollection<ReviewPreviewItem> Reviews { get; } = [];

    protected override async Task LoadAsync()
    {
        Title = "Reviews";
        _currentPage = 0;
        _totalPages = 0;
        Reviews.Clear();
        CanLoadMore = false;
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

    private async Task FetchNextPageAsync()
    {
        var (items, _, totalPages) = await movieDetailService.GetReviewsPageAsync(
            movieDetailNav.MovieId, _currentPage + 1, PageSize);

        foreach (var item in items)
            Reviews.Add(item);

        _currentPage++;
        _totalPages = totalPages;
        CanLoadMore = _currentPage < _totalPages;
    }

    [RelayCommand]
    private async Task ToggleLike(ReviewPreviewItem review)
    {
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
}
