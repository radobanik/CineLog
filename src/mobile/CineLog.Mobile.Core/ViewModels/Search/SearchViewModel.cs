using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models.Home;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Search;

public partial class SearchViewModel : BaseViewModel
{
    private readonly ISearchService _searchService;
    private CancellationTokenSource? _searchCts;
    private int _currentPage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowEmptyState))]
    [NotifyPropertyChangedFor(nameof(HasQuery))]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoResults))]
    [NotifyPropertyChangedFor(nameof(HasResults))]
    private bool _hasMovies;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowNoResults))]
    private bool _hasSearched;

    [ObservableProperty]
    private bool _isLoadingMore;

    [ObservableProperty]
    private bool _canLoadMore;

    public bool ShowEmptyState => string.IsNullOrWhiteSpace(SearchQuery);
    public bool HasQuery => !string.IsNullOrWhiteSpace(SearchQuery);
    public bool HasResults => HasMovies;
    public bool ShowNoResults => HasSearched && !IsBusy && !HasResults && !ShowEmptyState;

    public ObservableCollection<MovieItem> Movies { get; } = [];

    public SearchViewModel(ISearchService searchService, IAlertService alerts) : base(alerts)
    {
        _searchService = searchService;
        Title = "Search";
    }

    partial void OnSearchQueryChanged(string value)
    {
        _ = PerformSearchAsync(value);
    }

    [RelayCommand]
    private void Clear()
    {
        SearchQuery = string.Empty;
    }

    [RelayCommand]
    private async Task LoadMore()
    {
        if (IsLoadingMore || !CanLoadMore || IsBusy) return;

        try
        {
            IsLoadingMore = true;
            _currentPage++;
            var (movies, hasMore) = await _searchService.SearchMoviesAsync(SearchQuery, _currentPage);

            foreach (var movie in movies)
                Movies.Add(movie);

            CanLoadMore = hasMore;
        }
        catch (Exception ex)
        {
            _currentPage--;
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoadingMore = false;
        }
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
                Movies.Clear();
                HasMovies = false;
                HasSearched = false;
                CanLoadMore = false;
                _currentPage = 0;
                return;
            }

            IsBusy = true;
            _currentPage = 1;
            var (movies, hasMore) = await _searchService.SearchMoviesAsync(query, _currentPage, cts.Token);

            Movies.Clear();
            foreach (var movie in movies)
                Movies.Add(movie);

            HasMovies = Movies.Count > 0;
            CanLoadMore = hasMore;
            HasSearched = true;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            if (!cts.IsCancellationRequested)
                IsBusy = false;
        }
    }
}
