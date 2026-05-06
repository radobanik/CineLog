using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Common
{
    public sealed class UserDefaultsService(IAppDbContext context) : IUserDefaultsService
    {
        public async Task EnsureDefaultsAsync(Guid userId, CancellationToken ct = default)
        {
            foreach (var defaultWatchlist in DefaultWatchlists.All)
            {
                var exists = await context.Watchlists.AnyAsync(w =>
                    w.UserId == userId &&
                    w.Type == defaultWatchlist.Type,
                    ct);

                if (!exists)
                {
                    await context.Watchlists.AddAsync(
                        Watchlist.CreateDefault(userId, defaultWatchlist.Name, defaultWatchlist.Type),
                        ct);
                }
            }

            await context.SaveChangesAsync(ct);
        }
    }

    public static class DefaultWatchlists
    {
        public static readonly IReadOnlyList<(WatchlistType Type, string Name)> All =
        [
            (WatchlistType.Watched, "Watched"),
        (WatchlistType.WatchLater, "Watch later")
        ];
    }
}
