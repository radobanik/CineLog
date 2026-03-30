using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class UserSeeder
{
    private record SeedUser(string Username, string Email, string Password, string Role);

    private static readonly SeedUser[] Users =
    [
        new("admin_alice", "alice@cinelog.dev", "Admin1234!", UserRoles.Admin),
        new("admin_bob",   "bob@cinelog.dev",   "Admin1234!", UserRoles.Admin),
        new("user_carol",  "carol@cinelog.dev", "User1234!",  UserRoles.User),
        new("user_dave",   "dave@cinelog.dev",  "User1234!",  UserRoles.User),
    ];

    internal static async Task SeedAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var roleName in new[] { UserRoles.Admin, UserRoles.User })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        }

        foreach (var seed in Users)
        {
            if (await userManager.FindByEmailAsync(seed.Email) is not null)
                continue;

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = seed.Username,
                Email = seed.Email,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await userManager.CreateAsync(user, seed.Password);
            await userManager.AddToRoleAsync(user, seed.Role);
        }
    }
}
