using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class FavoriteSeeder
{
    private record SeedFavorite(string UserEmail, string MovieTitle);

    private static readonly SeedFavorite[] Favorites =
    [
        new("alice@cinelog.dev", "Fight Club"),
        new("alice@cinelog.dev", "Inception"),
        new("bob@cinelog.dev", "The Dark Knight"),
        new("carol@cinelog.dev", "Pulp Fiction"),
        new("dave@cinelog.dev", "Forrest Gump"),
        new("carol@cinelog.dev", "Fight Club")
    ];

    internal static async Task SeedAsync(IAppDbContext context)
    {
        var users = await context.Users.ToListAsync();
        var movies = await context.Movies.ToListAsync();

        if (users.Count == 0 || movies.Count == 0)
            return;

        foreach (var seed in Favorites)
        {
            var user = users.FirstOrDefault(u => u.Email == seed.UserEmail);
            var movie = movies.FirstOrDefault(m => m.Title == seed.MovieTitle);

            if (user == null || movie == null)
                continue;

            var exists = await context.UserFavorites.AnyAsync(f => f.UserId == user.Id && f.MovieId == movie.Id);
            if (exists)
                continue;

            var favorite = UserFavorite.Create(user.Id, movie.Id);
            context.UserFavorites.Add(favorite);
        }

        await context.SaveChangesAsync();
    }
}
