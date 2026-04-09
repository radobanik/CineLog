using CineLog.Domain.Interfaces;
using CineLog.Domain.ValueObjects;
using CineLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class ReviewSeeder
{
    private record SeedReview(string UserEmail, string MovieTitle, decimal Rating, string ReviewText, bool ContainsSpoilers);

    private static readonly SeedReview[] Reviews =
    [
        new("alice@cinelog.dev", "Fight Club", 4.5m, "First rule is you don't talk about it.", false),
        new("bob@cinelog.dev", "Inception", 4.0m, "A mind-bending masterpiece.", false),
        new("carol@cinelog.dev", "The Dark Knight", 5.0m, "Heath Ledger's Joker is phenomenal.", false),
        new("dave@cinelog.dev", "Pulp Fiction", 4.0m, "Classic Tarantino.", false),
        new("alice@cinelog.dev", "Inception", 5.0m, "Dream within a dream.", false)
    ];

    internal static async Task SeedAsync(IAppDbContext context)
    {
        var users = await context.Users.ToListAsync();
        var movies = await context.Movies.ToListAsync();

        if (users.Count == 0 || movies.Count == 0)
            return;

        foreach (var seed in Reviews)
        {
            var user = users.FirstOrDefault(u => u.Email == seed.UserEmail);
            var movie = movies.FirstOrDefault(m => m.Title == seed.MovieTitle);

            if (user == null || movie == null)
                continue;

            var exists = await context.Reviews.AnyAsync(r => r.UserId == user.Id && r.MovieId == movie.Id);
            if (exists)
                continue;

            var review = Review.Create(user.Id, movie.Id, Rating.Create(seed.Rating), seed.ReviewText, seed.ContainsSpoilers);
            context.Reviews.Add(review);
        }

        await context.SaveChangesAsync();
    }
}
