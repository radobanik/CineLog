using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Search;

public partial class SearchViewModel : BaseViewModel
{
    private CancellationTokenSource? searchCts;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasQuery))]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMoviesSelected))]
    [NotifyPropertyChangedFor(nameof(IsPeopleSelected))]
    private SearchSection selectedSection = SearchSection.Movies;

    public MovieSearchTabViewModel Movies { get; }
    public UserSearchTabViewModel People { get; }

    public bool HasQuery => !string.IsNullOrWhiteSpace(SearchQuery);
    public bool IsMoviesSelected => SelectedSection == SearchSection.Movies;
    public bool IsPeopleSelected => SelectedSection == SearchSection.People;

    public SearchViewModel(
        MovieSearchTabViewModel movies,
        UserSearchTabViewModel people,
        IAlertService alerts) : base(alerts)
    {
        Movies = movies;
        People = people;
        Title = "Search";
    }

    public override async Task OnAppearingAsync()
    {
        if (IsMoviesSelected)
            await Movies.SearchAsync(SearchQuery);
        else
            await People.SearchAsync(SearchQuery);
    }

    partial void OnSearchQueryChanged(string value) => _ = SearchDebouncedAsync(value);
    partial void OnSelectedSectionChanged(SearchSection value) => _ = SearchDebouncedAsync(SearchQuery);

    [RelayCommand] private void SelectMovies() => SelectedSection = SearchSection.Movies;
    [RelayCommand] private void SelectPeople() => SelectedSection = SearchSection.People;
    [RelayCommand] private void Clear() => SearchQuery = string.Empty;

    private async Task SearchDebouncedAsync(string query)
    {
        searchCts?.Cancel();
        searchCts?.Dispose();
        searchCts = new CancellationTokenSource();

        var ct = searchCts.Token;

        try
        {
            await Task.Delay(350, ct);

            if (IsMoviesSelected)
                await Movies.SearchAsync(query, ct);
            else
                await People.SearchAsync(query, ct);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
