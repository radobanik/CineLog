using CineLog.Mobile.ApiClient.Clients;
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
    private double _rating = 2.5;

    [ObservableProperty] private MovieDetailInfo? _movie;
    [ObservableProperty] private string? _reviewText;

    public bool CanDecrease => Rating > MinRating;
    public bool CanIncrease => Rating < MaxRating;
    public string RatingText => Rating.ToString("0.0");

    protected override async Task LoadAsync()
    {
        Title = "Add Review";
        Movie = await movieDetailService.GetMovieDetailAsync(movieDetailNav.MovieId);
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
        await ExecuteAsync(async () =>
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
        });
    }

    [RelayCommand]
    private async Task Cancel()
    {
        var confirmed = await alerts.ShowConfirmAsync(
            "Discard Review",
            "Are you sure you want to discard this review?",
            "Discard",
            "Keep Editing");

        if (confirmed)
            await navigation.NavigateBackAsync();
    }
}
