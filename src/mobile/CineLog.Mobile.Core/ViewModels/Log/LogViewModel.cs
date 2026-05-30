using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models.Activity;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Log;

public partial class LogViewModel : BaseViewModel
{
    private const int PageSize = 20;

    private readonly IActivityFeedService _activityFeedService;
    private readonly INavigationService _navigation;
    private readonly IMovieDetailNavigationContext _movieDetailNav;
    private readonly IReviewsNavigationContext _reviewsNav;

    private bool _canLoadMore = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoItems))]
    private bool _hasItems;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoItems))]
    private bool _hasLoadedOnce;

    [ObservableProperty]
    private bool _isLoadingMore;

    [ObservableProperty]
    private bool _isInitialLoading;

    public bool HasNoItems => !HasItems && HasLoadedOnce && !IsBusy;

    public ObservableCollection<ActivityFeedItem> Items { get; } = [];

    public LogViewModel(
        IActivityFeedService activityFeedService,
        INavigationService navigation,
        IMovieDetailNavigationContext movieDetailNav,
        IReviewsNavigationContext reviewsNav,
        IAlertService alerts)
        : base(alerts)
    {
        _activityFeedService = activityFeedService;
        _navigation = navigation;
        _movieDetailNav = movieDetailNav;
        _reviewsNav = reviewsNav;
        Title = "Log";
    }

    public override Task OnAppearingAsync()
    {
        return IsBusy ? Task.CompletedTask : Load();
    }

    [RelayCommand]
    private async Task Load()
    {
        IsInitialLoading = !HasLoadedOnce;

        await ExecuteAsync(async () =>
        {
            _canLoadMore = true;
            Items.Clear();

            var items = await _activityFeedService.GetActivityFeedAsync(0, PageSize);

            foreach (var item in items)
                Items.Add(item);

            _canLoadMore = items.Count == PageSize;
            HasItems = Items.Count > 0;
            HasLoadedOnce = true;
            OnPropertyChanged(nameof(HasNoItems));
        });

        IsInitialLoading = false;
    }

    [RelayCommand]
    private async Task LoadMore()
    {
        if (IsBusy || IsLoadingMore || !_canLoadMore)
            return;

        IsLoadingMore = true;

        try
        {
            var items = await _activityFeedService.GetActivityFeedAsync(Items.Count, PageSize);

            foreach (var item in items)
            {
                if (Items.All(existing => existing.Id != item.Id))
                    Items.Add(item);
            }

            _canLoadMore = items.Count == PageSize;
            HasItems = Items.Count > 0;
            OnPropertyChanged(nameof(HasNoItems));
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

    [RelayCommand]
    private async Task OpenActivity(ActivityFeedItem? item)
    {
        if (item is null)
            return;

        if (item.ReviewId.HasValue && item.MovieId.HasValue)
        {
            _movieDetailNav.MovieId = item.MovieId.Value;
            _reviewsNav.FocusReviewId = item.ReviewId.Value;

            await _navigation.NavigateToAsync(Routes.MovieDetail);
            return;
        }

        if (item.MovieId.HasValue)
        {
            _movieDetailNav.MovieId = item.MovieId.Value;
            _reviewsNav.FocusReviewId = null;

            await _navigation.NavigateToAsync(Routes.MovieDetail);
        }
    }
}
