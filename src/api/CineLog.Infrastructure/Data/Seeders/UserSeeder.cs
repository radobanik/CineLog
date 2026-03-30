using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class UserSeeder
{
    private record SeedUser(string Username, string Email, string Password, UserRole Role);

    private static readonly SeedUser[] Users =
    [
        new("admin_alice", "alice@cinelog.dev", "Admin1234!", UserRole.Admin),
        new("admin_bob",   "bob@cinelog.dev",   "Admin1234!", UserRole.Admin),
        new("user_carol",  "carol@cinelog.dev", "User1234!",  UserRole.User),
        new("user_dave",   "dave@cinelog.dev",  "User1234!",  UserRole.User),
    ];

    internal static void Seed(AppDbContext context)
    {
        foreach (var seed in Users)
        {
            if (context.Users.Any(u => u.Email == seed.Email))
                continue;

            var user = User.Create(seed.Username, seed.Email, BCrypt.Net.BCrypt.HashPassword(seed.Password));
            if (seed.Role == UserRole.Admin)
                user.AssignRole(UserRole.Admin);

            context.Users.Add(user);
        }

        context.SaveChanges();
    }

    internal static async Task SeedAsync(AppDbContext context, CancellationToken ct)
    {
        foreach (var seed in Users)
        {
            if (await context.Users.AnyAsync(u => u.Email == seed.Email, ct))
                continue;

            var user = User.Create(seed.Username, seed.Email, BCrypt.Net.BCrypt.HashPassword(seed.Password));
            if (seed.Role == UserRole.Admin)
                user.AssignRole(UserRole.Admin);

            await context.Users.AddAsync(user, ct);
        }

        await context.SaveChangesAsync(ct);
    }
}
