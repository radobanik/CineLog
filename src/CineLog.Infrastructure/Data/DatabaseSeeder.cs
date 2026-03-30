using CineLog.Infrastructure.Data.Seeders;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static void Seed(DbContext context, bool _)
    {
        var db = (AppDbContext)context;
        UserSeeder.Seed(db);
    }

    public static async Task SeedAsync(DbContext context, bool _, CancellationToken ct)
    {
        var db = (AppDbContext)context;
        await UserSeeder.SeedAsync(db, ct);
    }
}
