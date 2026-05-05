using System.Collections.ObjectModel;
using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Movies;

public partial class AddReviewViewModel : BaseViewModel
{
    private readonly ISearchService _searchService;
    private readonly IReviewsClient _reviewsClient;
    private readonly INavigationService _navigation;
    private readonly IAlertService _alerts;
    private CancellationTokenSource? _searchCts;

    // FA Solid: fa-star = U+F005, fa-star-half = U+F089
    private const string FaStar = "";
    private const string FaStarHalf = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedMovie))]
    [NotifyPropertyChangedFor(nameof(HasNoSelectedMovie))]
    private MovieItem? _selectedMovie;

    [ObservableProperty] private string _movieSearchQuery = string.Empty;
    [ObservableProperty] private bool _showMovieResults;
    [ObservableProperty] private string _reviewText = string.Empty;

    public ObservableCollection<MovieItem> MovieSearchResults { get; } = [];

    private double _rating;
    public double Rating
    {
        get => _rating;
        set
        {
            var snapped = Math.Round(value * 2) / 2.0;
            snapped = Math.Clamp(snapped, 0, 5);
            SetProperty(ref _rating, snapped);
            OnPropertyChanged(nameof(RatingText));
            OnPropertyChanged(nameof(StarDisplay));
        }
    }

    public bool HasSelectedMovie => SelectedMovie is not null;
    public bool HasNoSelectedMovie => SelectedMovie is null;

    public string RatingText => Rating == 0 ? "No rating" : $"{Rating:0.#} / 5";

    public string StarDisplay
    {
        get
        {
            var full = (int)Math.Floor(Rating);
            var hasHalf = (Rating - full) >= 0.5;
            return string.Concat(Enumerable.Repeat(FaStar, full)) + (hasHalf ? FaStarHalf : string.Empty);
        }
    }

    public AddReviewViewModel(
        ISearchService searchService,
        IReviewsClient reviewsClient,
        INavigationService navigation,
        IAlertService alerts) : base(alerts)
    {
        _searchService = searchService;
        _reviewsClient = reviewsClient;
        _navigation = navigation;
        _alerts = alerts;
    }

    protected override Task LoadAsync()
    {
        Title = "Log a Film";
        return Task.CompletedTask;
    }

    partial void OnMovieSearchQueryChanged(string value) => _ = PerformSearchAsync(value);

    [RelayCommand]
    private void SelectMovie(MovieItem movie)
    {
        SelectedMovie = movie;
        MovieSearchQuery = string.Empty;
        MovieSearchResults.Clear();
        ShowMovieResults = false;
    }

    [RelayCommand]
    private void ClearMovie()
    {
        SelectedMovie = null;
        MovieSearchQuery = string.Empty;
    }

    [RelayCommand]
    private async Task Submit()
    {
        if (SelectedMovie is null)
        {
            await _alerts.ShowAlertAsync("Missing info", "Please select a movie to review.");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var title = SelectedMovie.Title;
            await _reviewsClient.CreateAsync(new CreateReviewCommand
            {
                MovieId = SelectedMovie.Id,
                Rating = Rating == 0 ? null : Rating,
                ReviewText = string.IsNullOrWhiteSpace(ReviewText) ? null : ReviewText.Trim(),
                ContainsSpoilers = false
            });
            ResetForm();
            await _alerts.ShowToastAsync($"Logged \"{title}\"");
        });
    }

    private void ResetForm()
    {
        SelectedMovie = null;
        MovieSearchQuery = string.Empty;
        MovieSearchResults.Clear();
        ShowMovieResults = false;
        Rating = 0;
        ReviewText = string.Empty;
    }

    [RelayCommand]
    private async Task Cancel()
    {
        var confirmed = await _alerts.ShowConfirmAsync(
            "Discard review?",
            "Are you sure you want to cancel?",
            "Yes", "No");
        if (confirmed)
            await _navigation.NavigateToRootAsync(Routes.AuthenticatedRoot);
    }

    private async Task PerformSearchAsync(string query)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var cts = _searchCts;

        try
        {
            await Task.Delay(400, cts.Token);

            if (string.IsNullOrWhiteSpace(query))
            {
                MovieSearchResults.Clear();
                ShowMovieResults = false;
                return;
            }

            var (movies, _) = await _searchService.SearchMoviesAsync(query, 1, cts.Token);

            MovieSearchResults.Clear();
            foreach (var movie in movies.Take(5))
                MovieSearchResults.Add(movie);

            ShowMovieResults = MovieSearchResults.Count > 0;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
