using CineLog.Domain.Entities;
using CineLog.Application.Common;
using CineLog.Infrastructure.Data.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CineLog.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager  = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager  = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var blobStorage  = serviceProvider.GetRequiredService<IBlobStorageService>();
        await UserSeeder.SeedAsync(userManager, roleManager, blobStorage);
    }
}
