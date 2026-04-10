using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Infrastructure.Data.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CineLog.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var blobStorage = serviceProvider.GetRequiredService<IBlobStorageService>();
        await UserSeeder.SeedAsync(userManager, roleManager, blobStorage);

        var dbContext = serviceProvider.GetRequiredService<CineLog.Domain.Interfaces.IAppDbContext>();
        await MovieSeeder.SeedAsync(dbContext);
        await ReviewSeeder.SeedAsync(dbContext);
        await FavoriteSeeder.SeedAsync(dbContext);
    }
}
