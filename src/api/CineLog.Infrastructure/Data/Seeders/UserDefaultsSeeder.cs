using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class UserDefaultsSeeder
{
    internal static async Task SeedAsync(
        IAppDbContext context,
        IUserDefaultsService userDefaults)
    {
        var users = await context.Users
            .AsNoTracking()
            .Select(u => u.Id)
            .ToListAsync();

        foreach (var userId in users)
            await userDefaults.EnsureDefaultsAsync(userId);
    }
}
