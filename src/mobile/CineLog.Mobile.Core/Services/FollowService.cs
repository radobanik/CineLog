using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Models.Search;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class FollowService(IUsersClient usersClient) : IFollowService
{
    private const int PageSize = 12;

    public async Task<PagedResult<UserSearchItem>> GetFollowingAsync(
        int page,
        CancellationToken ct = default)
    {
        var me = await usersClient.GetMeAsync(ct);
        var result = await usersClient.GetFollowingAsync(me.Id ?? Guid.Empty, page, PageSize, ct);

        return new PagedResult<UserSearchItem>(
            MapFollowingUsers(result.Items),
            page < (result.TotalPages ?? 1));
    }

    public Task FollowAsync(Guid userId, CancellationToken ct = default) =>
        usersClient.FollowAsync(userId, ct);

    public Task UnfollowAsync(Guid userId, CancellationToken ct = default) =>
        usersClient.UnfollowAsync(userId, ct);

    private static IReadOnlyList<UserSearchItem> MapFollowingUsers(
        IEnumerable<UserSummaryResponse>? users)
    {
        return users?.Select(u => new UserSearchItem
        {
            Id = u.Id ?? Guid.Empty,
            Username = u.Username ?? string.Empty,
            AvatarUrl = u.AvatarUrl,
            IsFollowing = true,
            ReviewCount = u.ReviewCount ?? 0
        }).ToList() ?? [];
    }
}
