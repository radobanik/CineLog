using System.Collections.ObjectModel;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;
using CineLog.Mobile.Core.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CineLog.Mobile.Core.Navigation;

namespace CineLog.Mobile.Core.ViewModels.Search;

public partial class UserSearchTabViewModel(
    IUserService userService,
    IFollowService followService,
    INavigationService navigation,
    IAlertService alerts) : BaseViewModel(alerts)
{
    private string currentQuery = string.Empty;
    private int searchPage;
    private int followingPage;
    private bool searchHasMore;
    private bool followingHasMore;

    [ObservableProperty] private bool hasQuery;
    [ObservableProperty] private bool showSkeleton;
    [ObservableProperty] private bool showNoResults;
    [ObservableProperty] private bool isLoadingMore;

    public ObservableCollection<UserSearchRowViewModel> SearchResults { get; } = [];
    public ObservableCollection<UserSearchRowViewModel> RecommendedUsers { get; } = [];
    public ObservableCollection<UserSearchRowViewModel> FollowingUsers { get; } = [];

    public bool ShowResults => HasQuery && SearchResults.Count > 0;
    public bool ShowHome => !HasQuery;
    public bool CanLoadMore => HasQuery ? searchHasMore : followingHasMore;

    public async Task SearchAsync(string query, CancellationToken ct = default)
    {
        HasQuery = !string.IsNullOrWhiteSpace(query);
        ShowNoResults = false;
        SearchResults.Clear();

        if (!HasQuery)
        {
            await LoadHomeAsync(ct);
            RefreshVisibility();
            return;
        }

        currentQuery = query.Trim();
        searchPage = 1;
        searchHasMore = false;
        ShowSkeleton = true;

        try
        {
            var result = await userService.SearchUsersAsync(currentQuery, searchPage, ct);
            AddUsers(SearchResults, result.Items);
            searchHasMore = result.HasMore;
            ShowNoResults = SearchResults.Count == 0;
        }
        finally
        {
            if (!ct.IsCancellationRequested)
                ShowSkeleton = false;

            RefreshVisibility();
        }
    }

    public async Task LoadHomeAsync(CancellationToken ct = default)
    {
        RecommendedUsers.Clear();
        AddUsers(RecommendedUsers, await userService.GetRecommendedUsersAsync(10, ct));

        FollowingUsers.Clear();
        followingPage = 1;
        var result = await followService.GetFollowingAsync(followingPage, ct);
        AddUsers(FollowingUsers, result.Items);
        followingHasMore = result.HasMore;

    }

    [RelayCommand]
    private async Task LoadMore()
    {
        if (IsLoadingMore || !CanLoadMore)
            return;

        IsLoadingMore = true;

        try
        {
            if (HasQuery)
                await LoadMoreSearchResultsAsync();
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

        try
        {
            if (user.IsFollowing)
            {
                await followService.UnfollowAsync(user.Id);
                SetFollowingState(user.Id, false);
                RemoveUser(FollowingUsers, user.Id);
                RefreshVisibility();
                return;
            }

            await followService.FollowAsync(user.Id);
            SetFollowingState(user.Id, true);

            if (FollowingUsers.All(x => x.Id != user.Id))
                FollowingUsers.Insert(0, CloneRow(user, isFollowing: true));

            RefreshVisibility();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }


    [RelayCommand]
    private Task OpenProfile(UserSearchRowViewModel? user)
    {
        if (user is null || user.Id == Guid.Empty)
            return Task.CompletedTask;

        return navigation.NavigateToAsync(
            Routes.UserProfile,
            new Dictionary<string, object> { ["userId"] = user.Id });
    }

    private async Task LoadMoreSearchResultsAsync()
    {
        var result = await userService.SearchUsersAsync(currentQuery, ++searchPage);
        AddUsers(SearchResults, result.Items);
        searchHasMore = result.HasMore;
        RefreshVisibility();
    }

    private async Task LoadMoreFollowingAsync()
    {
        var result = await followService.GetFollowingAsync(++followingPage);
        AddUsers(FollowingUsers, result.Items);
        followingHasMore = result.HasMore;
        RefreshVisibility();
    }

    private void SetFollowingState(Guid userId, bool isFollowing)
    {
        foreach (var user in SearchResults
            .Concat(RecommendedUsers)
            .Concat(FollowingUsers)
            .Where(x => x.Id == userId))
        {
            user.IsFollowing = isFollowing;
        }
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
        bool isFollowing) =>
        new(new UserSearchItem
        {
            Id = row.Id,
            Username = row.Username,
            AvatarUrl = row.AvatarUrl,
            ReviewCount = row.ReviewCount,
            IsFollowing = isFollowing
        });

    private void RefreshVisibility()
    {
        OnPropertyChanged(nameof(ShowResults));
        OnPropertyChanged(nameof(ShowHome));
        OnPropertyChanged(nameof(CanLoadMore));
    }
}
