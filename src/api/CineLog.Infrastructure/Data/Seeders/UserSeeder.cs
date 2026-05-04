using System.Reflection;
using CineLog.Application.Common;
using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class UserSeeder
{
    private record SeedUser(string Username, string Email, string Password, string Bio, string Role, string? AvatarFile = "avatar_admin_alice.png");

    private static readonly SeedUser[] Users =
    [
        new("admin_alice", "alice@cinelog.dev", "Admin1234!", "Obsessive movie buff with a passion for 70s cult classics.", UserRoles.Admin, "avatar_admin_alice.png"),
        new("admin_bob",   "bob@cinelog.dev",   "Admin1234!", "Harsh rater. I value original screenplays over big budgets.", UserRoles.Admin, "avatar_admin_bob.png"),
        new("user_carol",  "carol@cinelog.dev", "User1234!",  "Horror junkie and sci-fi nerd. Always hunting for gore.", UserRoles.User,  "avatar_admin_carol.png"),
        new("user_dave",   "dave@cinelog.dev",  "User1234!",  "Goal: Watch every film in the IMDb Top 250.", UserRoles.User,  "avatar_admin_dave.png"),

        new("lucy_films", "lucy@cinelog.dev", "User1234!", "Writes short reviews after every late-night screening.", UserRoles.User),
        new("karsten_r", "karsten@cinelog.dev", "User1234!", "European cinema, crime dramas, and slow-burn thrillers.", UserRoles.User),
        new("mila_reels", "mila@cinelog.dev", "User1234!", "Always looking for great production design.", UserRoles.User),
        new("noah_cuts", "noah@cinelog.dev", "User1234!", "Editing nerd. I notice rhythm before plot.", UserRoles.User),
        new("sofia_scope", "sofia@cinelog.dev", "User1234!", "Romance, animation, and anything with a strong score.", UserRoles.User),
        new("leo_frame", "leo@cinelog.dev", "User1234!", "Mostly watches classics and neo-noir.", UserRoles.User),
        new("emma_logs", "emma@cinelog.dev", "User1234!", "Daily diary, generous ratings, honest notes.", UserRoles.User),
        new("mateo_movies", "mateo@cinelog.dev", "User1234!", "Action, martial arts, and practical effects.", UserRoles.User),
        new("nina_nights", "nina@cinelog.dev", "User1234!", "Festival picks and quiet character studies.", UserRoles.User),
        new("oscar_views", "oscar@cinelog.dev", "User1234!", "I rank everything. Sometimes twice.", UserRoles.User),
        new("ivy_reviews", "ivy@cinelog.dev", "User1234!", "Documentaries, mysteries, and strong endings.", UserRoles.User),
        new("tom_projector", "tom@cinelog.dev", "User1234!", "Big screen believer. Popcorn optional.", UserRoles.User)
    ];

    internal static async Task SeedAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IBlobStorageService blobStorage,
        IUserDefaultsService userDefaults)
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
                Bio = seed.Bio,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await userManager.CreateAsync(user, seed.Password);
            await userManager.AddToRoleAsync(user, seed.Role);
            await userDefaults.EnsureDefaultsAsync(user.Id);

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
