using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Navigation;
using CineLog.Mobile.Core.Services;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CineLog.Mobile.Core.ViewModels.Search;

public partial class SearchViewModel : BaseViewModel
{
    private readonly ISearchService searchService;
    private readonly IUserService userService;
    private readonly IFollowService followService;
    private readonly IMovieDetailNavigationContext movieDetailNav;
    private readonly INavigationService navigation;

    private CancellationTokenSource? _searchCts;
    private int _moviePage;
    private int _userPage;
    private int _followingPage;

    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private SearchSection _selectedSection = SearchSection.Movies;
    [ObservableProperty] private bool _showSkeleton;
    [ObservableProperty] private bool _hasQuery;
    [ObservableProperty] private bool _showNoResults;
    [ObservableProperty] private bool _isLoadingMore;
    [ObservableProperty] private bool _canLoadMore;

    public ObservableCollection<MovieItem> Movies { get; } = [];
    public ObservableCollection<UserSearchRowViewModel> Users { get; } = [];
    public ObservableCollection<UserSearchRowViewModel> RecommendedUsers { get; } = [];
    public ObservableCollection<UserSearchRowViewModel> FollowingUsers { get; } = [];

    public bool IsMoviesSelected => SelectedSection == SearchSection.Movies;
    public bool IsPeopleSelected => SelectedSection == SearchSection.People;

    public bool ShowMovieResults => IsMoviesSelected && HasQuery && Movies.Count > 0;
    public bool ShowPeopleResults => IsPeopleSelected && HasQuery && Users.Count > 0;
    public bool ShowPeopleHome => IsPeopleSelected && !HasQuery;
    public bool ShowMovieEmptyState => IsMoviesSelected && !HasQuery;

    public SearchViewModel(
        ISearchService searchService,
        IUserService userService,
        IFollowService followService,
        IMovieDetailNavigationContext movieDetailNav,
        INavigationService navigation,
        IAlertService alerts) : base(alerts)
    {
        this.searchService = searchService;
        this.userService = userService;
        this.followService = followService;
        this.movieDetailNav = movieDetailNav;
        this.navigation = navigation;
        Title = "Search";
    }

    public override async Task OnAppearingAsync()
    {
        if (SelectedSection == SearchSection.People && !HasQuery)
            await LoadPeopleHomeAsync();
    }

    [RelayCommand]
    public Task GoToMovie(MovieItem movie)
    {
        movieDetailNav.MovieId = movie.Id;
        return navigation.NavigateToAsync(Routes.MovieDetail);
    }

    partial void OnSearchQueryChanged(string value)
    {
        HasQuery = !string.IsNullOrWhiteSpace(value);
        _ = SearchDebouncedAsync(value);
        RefreshVisibility();
    }

    partial void OnSelectedSectionChanged(SearchSection value)
    {
        OnPropertyChanged(nameof(IsMoviesSelected));
        OnPropertyChanged(nameof(IsPeopleSelected));

        _ = HasQuery
            ? SearchDebouncedAsync(SearchQuery)
            : LoadPeopleHomeAsync();

        RefreshVisibility();
    }

    [RelayCommand]
    private void SelectMovies() => SelectedSection = SearchSection.Movies;

    [RelayCommand]
    private void SelectPeople() => SelectedSection = SearchSection.People;

    [RelayCommand]
    private void Clear() => SearchQuery = string.Empty;

    [RelayCommand]
    private async Task LoadMore()
    {
        if (IsBusy || IsLoadingMore || !CanLoadMore)
            return;

        try
        {
            IsLoadingMore = true;

            if (IsMoviesSelected)
                await LoadMoreMoviesAsync();
            else if (HasQuery)
                await LoadMoreUsersAsync();
            else
                await LoadMoreFollowingAsync();
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    [RelayCommand]
    private async Task ToggleFollow(UserSearchRowViewModel user)
    {
        if (user.Id == Guid.Empty)
            return;

        if (user.IsFollowing)
        {
            await followService.UnfollowAsync(user.Id);
            SetFollowingState(user.Id, false);
            RemoveUser(FollowingUsers, user.Id);
            return;
        }

        await followService.FollowAsync(user.Id);
        SetFollowingState(user.Id, true);

        if (FollowingUsers.All(x => x.Id != user.Id))
            FollowingUsers.Insert(0, CloneRow(user, isFollowing: true));
    }

    private async Task SearchDebouncedAsync(string query)
    {
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var cts = _searchCts;

        try
        {
            await Task.Delay(350, cts.Token);

            if (string.IsNullOrWhiteSpace(query))
            {
                Movies.Clear();
                Users.Clear();
                CanLoadMore = false;
                ShowNoResults = false;

                if (IsPeopleSelected)
                    await LoadPeopleHomeAsync(cts.Token);

                RefreshVisibility();
                return;
            }

            ShowSkeleton = true;
            ShowNoResults = false;

            if (IsMoviesSelected)
                await SearchMoviesAsync(query, cts.Token);
            else
                await SearchUsersAsync(query, cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            if (!cts.IsCancellationRequested)
                ShowSkeleton = false;

            RefreshVisibility();
        }
    }

    private async Task SearchMoviesAsync(string query, CancellationToken ct)
    {
        _moviePage = 1;
        var (movies, hasMore) = await searchService.SearchMoviesAsync(query, _moviePage, ct);

        Movies.Clear();
        foreach (var movie in movies)
            Movies.Add(movie);

        CanLoadMore = hasMore;
        ShowNoResults = Movies.Count == 0;
    }

    private async Task SearchUsersAsync(string query, CancellationToken ct)
    {
        _userPage = 1;
        var (users, hasMore) = await userService.SearchUsersAsync(query, _userPage, ct);

        Users.Clear();
        AddUsers(Users, users);

        CanLoadMore = hasMore;
        ShowNoResults = Users.Count == 0;
    }

    private async Task LoadPeopleHomeAsync(CancellationToken ct = default)
    {
        if (!IsPeopleSelected || HasQuery)
            return;

        if (RecommendedUsers.Count == 0)
            AddUsers(RecommendedUsers, await userService.GetRecommendedUsersAsync(10, ct));

        if (FollowingUsers.Count == 0)
        {
            _followingPage = 1;
            var (users, hasMore) = await followService.GetFollowingAsync(_followingPage, ct);
            AddUsers(FollowingUsers, users);
            CanLoadMore = hasMore;
        }
    }

    private async Task LoadMoreMoviesAsync()
    {
        _moviePage++;
        var (movies, hasMore) = await searchService.SearchMoviesAsync(SearchQuery, _moviePage);

        foreach (var movie in movies.Where(m => Movies.All(x => x.Id != m.Id)))
            Movies.Add(movie);

        CanLoadMore = hasMore;
    }

    private async Task LoadMoreUsersAsync()
    {
        _userPage++;
        var (users, hasMore) = await userService.SearchUsersAsync(SearchQuery, _userPage);

        AddUsers(Users, users);
        CanLoadMore = hasMore;
    }

    private async Task LoadMoreFollowingAsync()
    {
        _followingPage++;
        var (users, hasMore) = await followService.GetFollowingAsync(_followingPage);

        AddUsers(FollowingUsers, users);
        CanLoadMore = hasMore;
    }

    private void SetFollowingState(Guid userId, bool isFollowing)
    {
        foreach (var user in Users.Concat(RecommendedUsers).Concat(FollowingUsers).Where(x => x.Id == userId))
            user.IsFollowing = isFollowing;
    }

    private static void AddUsers(
        ObservableCollection<UserSearchRowViewModel> target,
        IEnumerable<UserSearchItem> users)
    {
        var existingIds = target.Select(x => x.Id).ToHashSet();

        foreach (var user in users.Where(x => existingIds.Add(x.Id)))
            target.Add(new UserSearchRowViewModel(user));
    }

    private static void RemoveUser(
        ObservableCollection<UserSearchRowViewModel> target,
        Guid userId)
    {
        var row = target.FirstOrDefault(x => x.Id == userId);
        if (row is not null)
            target.Remove(row);
    }

    private static UserSearchRowViewModel CloneRow(
        UserSearchRowViewModel row,
        bool isFollowing)
    {
        return new UserSearchRowViewModel(new UserSearchItem
        {
            Id = row.Id,
            Username = row.Username,
            AvatarUrl = row.AvatarUrl,
            ReviewCount = row.ReviewCount,
            IsFollowing = isFollowing
        });
    }

    private void RefreshVisibility()
    {
        OnPropertyChanged(nameof(ShowMovieResults));
        OnPropertyChanged(nameof(ShowPeopleResults));
        OnPropertyChanged(nameof(ShowPeopleHome));
        OnPropertyChanged(nameof(ShowMovieEmptyState));
    }
}
