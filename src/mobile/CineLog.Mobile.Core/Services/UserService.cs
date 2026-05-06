using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class UserService(ISearchClient searchClient,IUsersClient usersClient): IUserService
{
    private const int PageSize = 12;

    public async Task<PagedResult<UserSearchItem>> SearchUsersAsync(
        string query,
        int page,
        CancellationToken ct = default)
    {
        var result = await searchClient.SearchUsersAsync(query, page, PageSize, ct);

        return new PagedResult<UserSearchItem>(
            MapDiscoverUsers(result.Items),
            page < (result.TotalPages ?? 1));
    }

    public async Task<IReadOnlyList<UserSearchItem>> GetRecommendedUsersAsync(
        int limit,
        CancellationToken ct = default)
    {
        var result = await usersClient.GetRecommendedAsync(limit, ct);
        return MapDiscoverUsers(result);
    }

    private static IReadOnlyList<UserSearchItem> MapDiscoverUsers(
        IEnumerable<DiscoverUserResponse>? users)
    {
        return users?.Select(u => new UserSearchItem
        {
            Id = u.Id ?? Guid.Empty,
            Username = u.Username ?? string.Empty,
            AvatarUrl = u.AvatarUrl,
            ReviewCount = u.ReviewCount ?? 0,
            IsFollowing = u.IsFollowing ?? false
        }).ToList() ?? [];
    }
}
