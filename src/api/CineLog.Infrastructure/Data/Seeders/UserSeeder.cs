using System.Reflection;
using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class UserSeeder
{
    private record SeedUser(string Username, string Email, string Password, string Role, string AvatarFile);

    private static readonly SeedUser[] Users =
    [
        new("admin_alice", "alice@cinelog.dev", "Admin1234!", UserRoles.Admin, "avatar_admin_alice.png"),
        new("admin_bob",   "bob@cinelog.dev",   "Admin1234!", UserRoles.Admin, "avatar_admin_bob.png"),
        new("user_carol",  "carol@cinelog.dev", "User1234!",  UserRoles.User,  "avatar_admin_carol.png"),
        new("user_dave",   "dave@cinelog.dev",  "User1234!",  UserRoles.User,  "avatar_admin_dave.png"),
    ];

    internal static async Task SeedAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IBlobStorageService blobStorage)
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

            var avatarUrl = await UploadAvatarAsync(blobStorage, user.Id, seed.AvatarFile);
            if (avatarUrl is not null)
            {
                user.AvatarUrl = avatarUrl;
                await userManager.UpdateAsync(user);
            }
        }
    }

    private static async Task<string?> UploadAvatarAsync(
        IBlobStorageService blobStorage,
        Guid userId,
        string fileName)
    {
        var resourceName = $"CineLog.Infrastructure.Data.Seeders.Assets.UserAvatars.{fileName}";
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null)
            return null;

        await using (stream)
        {
            var key = $"avatars/{userId}.png";
            return await blobStorage.UploadAsync(key, stream, "image/png");
        }
    }
}
