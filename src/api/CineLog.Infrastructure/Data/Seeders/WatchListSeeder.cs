using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class WatchlistSeeder
{
    private record SeedWatchlist(string UserEmail, string Name, string[] MovieTitles);

    private static readonly SeedWatchlist[] Watchlists =
    [
        new("alice@cinelog.dev", "Mind-benders", ["Inception", "Fight Club", "The Matrix"]),
        new("bob@cinelog.dev", "Sci-fi canon", ["Blade Runner", "The Matrix", "Star Wars"]),
        new("carol@cinelog.dev", "Horror night", ["Get Out", "Midsommar", "The Shining"]),
        new("dave@cinelog.dev", "Fantasy marathon", [
            "The Lord of the Rings: The Fellowship of the Ring",
            "The Lord of the Rings: The Two Towers",
            "The Lord of the Rings: The Return of the King"
        ])
    ];

    internal static async Task SeedAsync(IAppDbContext context)
    {
        var users = await context.Users.ToListAsync();
        var movies = await context.Movies.ToListAsync();

        foreach (var seed in Watchlists)
        {
            var user = users.FirstOrDefault(u => u.Email == seed.UserEmail);
            if (user is null)
                continue;

            var watchlist = await context.Watchlists
                .FirstOrDefaultAsync(w => w.UserId == user.Id && w.Name == seed.Name);

            if (watchlist is null)
            {
                watchlist = Watchlist.Create(user.Id, seed.Name);
                context.Watchlists.Add(watchlist);
                await context.SaveChangesAsync();
            }

            foreach (var title in seed.MovieTitles)
            {
                var movie = movies.FirstOrDefault(m => m.Title == title);
                if (movie is null)
                    continue;

                var exists = await context.WatchlistItems
                    .AnyAsync(i => i.WatchlistId == watchlist.Id && i.MovieId == movie.Id);

                if (!exists)
                    context.WatchlistItems.Add(WatchlistItem.Create(watchlist.Id, movie.Id));
            }
        }

        await context.SaveChangesAsync();
    }
}
