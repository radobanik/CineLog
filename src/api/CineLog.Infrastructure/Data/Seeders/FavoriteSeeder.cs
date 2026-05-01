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
        new("alice@cinelog.dev", "Blade Runner"),
        new("alice@cinelog.dev", "The Matrix"),


        new("bob@cinelog.dev", "The Dark Knight"),
        new("bob@cinelog.dev", "Blade Runner"),
        new("bob@cinelog.dev", "Star Wars"),


        new("carol@cinelog.dev", "Pulp Fiction"),
        new("carol@cinelog.dev", "Fight Club"),
        new("carol@cinelog.dev", "Get Out"),
        new("carol@cinelog.dev", "Midsommar"),
        new("carol@cinelog.dev", "The Shining"),

        new("dave@cinelog.dev", "Forrest Gump"),
        new("dave@cinelog.dev", "The Lord of the Rings: The Fellowship of the Ring"),
        new("dave@cinelog.dev", "The Lord of the Rings: The Two Towers"),
        new("dave@cinelog.dev", "The Lord of the Rings: The Return of the King"),
        new("dave@cinelog.dev", "Spirited Away")
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
