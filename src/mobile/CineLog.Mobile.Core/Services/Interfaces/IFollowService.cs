using System;
using System.Collections.Generic;
using System.Text;
using CineLog.Mobile.Core.Models.Search;

namespace CineLog.Mobile.Core.Services.Interfaces
{
    public interface IFollowService
    {
        Task<(IReadOnlyList<UserSearchItem> Users, bool HasMore)> GetFollowingAsync(
            int page,
            CancellationToken ct = default);

        Task FollowAsync(Guid userId, CancellationToken ct = default);
        Task UnfollowAsync(Guid userId, CancellationToken ct = default);
    }
}
