using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Infrastructure;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Movies;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Movies;

public partial class AddReviewViewModel(
    IMovieDetailService movieDetailService,
    IMovieDetailNavigationContext movieDetailNav,
    IEditReviewNavigationContext editReviewNav,
    IReviewsClient reviewsClient,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    private const double MinRating = 0.5;
    private const double MaxRating = 5.0;
    private const double RatingStep = 0.5;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDecrease))]
    [NotifyPropertyChangedFor(nameof(CanIncrease))]
    [NotifyPropertyChangedFor(nameof(RatingText))]
    [NotifyPropertyChangedFor(nameof(Stars))]
    private double _rating = 0.0;

    [ObservableProperty] private MovieDetailInfo? _movie;
    [ObservableProperty] private string? _reviewText;

    private string _movieTitle = string.Empty;
    public string MovieTitle
    {
        get => _movieTitle;
        private set => SetProperty(ref _movieTitle, value);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get => _isEditMode;
        private set
        {
            if (SetProperty(ref _isEditMode, value))
            {
                OnPropertyChanged(nameof(IsNotEditMode));
                OnPropertyChanged(nameof(SubmitButtonText));
            }
        }
    }

    public bool IsNotEditMode => !IsEditMode;
    public string SubmitButtonText => IsEditMode ? "Update Review" : "Submit Review";

    public bool CanDecrease => Rating > MinRating;
    public bool CanIncrease => Rating < MaxRating;
    public string RatingText => Rating.ToString("0.0");

    public IReadOnlyList<StarDisplayItem> Stars => BuildStarItems(Rating);

    protected override async Task LoadAsync()
    {
        IsEditMode = editReviewNav.ReviewId != Guid.Empty;

        if (IsEditMode)
        {
            Title = "Edit Review";
            MovieTitle = editReviewNav.MovieTitle;
            Rating = editReviewNav.Rating;
            ReviewText = editReviewNav.ReviewText;
        }
        else
        {
            Title = "Add Review";
            Movie = await movieDetailService.GetMovieDetailAsync(movieDetailNav.MovieId);
            MovieTitle = Movie?.Title ?? string.Empty;
        }
    }

    [RelayCommand]
    private void IncreaseRating()
    {
        if (Rating < MaxRating)
            Rating = Math.Round(Rating + RatingStep, 1);
    }

    [RelayCommand]
    private void DecreaseRating()
    {
        if (Rating > MinRating)
            Rating = Math.Round(Rating - RatingStep, 1);
    }

    [RelayCommand]
    private async Task Submit()
    {
        if (Rating == 0.0)
        {
            await alerts.ShowAlertAsync("Rating Required", "Please select a star rating before submitting.");
            return;
        }

        await ExecuteAsync(async () =>
        {
            if (IsEditMode)
            {
                await reviewsClient.UpdateAsync(editReviewNav.ReviewId, new UpdateReviewCommand
                {
                    ReviewId = editReviewNav.ReviewId,
                    Rating = Rating,
                    ReviewText = string.IsNullOrWhiteSpace(ReviewText) ? null : ReviewText,
                    ContainsSpoilers = false
                });

                editReviewNav.ReviewId = Guid.Empty;
                await alerts.ShowToastAsync("Review updated!");
                await navigation.NavigateBackAsync();
            }
            else
            {
                try
                {
                    await reviewsClient.CreateAsync(new CreateReviewCommand
                    {
                        MovieId = movieDetailNav.MovieId,
                        Rating = Rating,
                        ReviewText = string.IsNullOrWhiteSpace(ReviewText) ? null : ReviewText,
                        ContainsSpoilers = false
                    });

                    await alerts.ShowToastAsync("Review submitted!");
                    await navigation.NavigateBackAsync();
                }
                catch (ApiException ex) when (ex.StatusCode == 409)
                {
                    await alerts.ShowAlertAsync("Already Reviewed", "You have already submitted a review for this film.");
                }
            }
        });
    }

    [RelayCommand]
    private async Task DeleteReview()
    {
        var confirmed = await alerts.ShowConfirmAsync(
            "Delete Review",
            "Are you sure you want to delete this review? This cannot be undone.",
            "Delete",
            "Cancel");

        if (!confirmed)
            return;

        await ExecuteAsync(async () =>
        {
            await reviewsClient.DeleteAsync(editReviewNav.ReviewId);
            editReviewNav.ReviewId = Guid.Empty;
            await alerts.ShowToastAsync("Review deleted.");
            await navigation.NavigateBackAsync();
        });
    }

    [RelayCommand]
    private async Task Cancel()
    {
        var confirmed = await alerts.ShowConfirmAsync(
            IsEditMode ? "Discard Changes" : "Discard Review",
            IsEditMode ? "Are you sure you want to discard your changes?" : "Are you sure you want to discard this review?",
            "Discard",
            "Keep Editing");

        if (confirmed)
        {
            if (IsEditMode)
                editReviewNav.ReviewId = Guid.Empty;
            await navigation.NavigateBackAsync();
        }
    }

    private static IReadOnlyList<StarDisplayItem> BuildStarItems(double rating)
    {
        var items = new StarDisplayItem[5];
        for (var i = 0; i < 5; i++)
        {
            if (rating >= i + 1.0)
                items[i] = new StarDisplayItem(IsFull: true, IsHalf: false);
            else if (rating >= i + 0.5)
                items[i] = new StarDisplayItem(IsFull: false, IsHalf: true);
            else
                items[i] = new StarDisplayItem(IsFull: false, IsHalf: false);
        }
        return items;
    }
}
