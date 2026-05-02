using System;
using System.Collections.Generic;
using CineLog.Mobile.Core.Models.Search;

namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IUserService
{
    Task<(IReadOnlyList<UserSearchItem> Users, bool HasMore)> SearchUsersAsync(
        string query,
        int page,
        CancellationToken ct = default);

    Task<IReadOnlyList<UserSearchItem>> GetRecommendedUsersAsync(
        int limit,
        CancellationToken ct = default);
}
