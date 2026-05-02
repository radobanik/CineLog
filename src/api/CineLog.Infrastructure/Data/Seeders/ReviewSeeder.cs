// src/api/CineLog.Infrastructure/Data/Seeders/ReviewSeeder.cs
using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using CineLog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class ReviewSeeder
{
    private record SeedReview(
        string UserEmail,
        string MovieTitle,
        decimal Rating,
        string ReviewText,
        bool ContainsSpoilers);

    private static readonly SeedReview[] FixedReviews =
    [
        new("alice@cinelog.dev", "Fight Club", 4.5m, "First rule is you don't talk about it.", false),
        new("bob@cinelog.dev", "Inception", 4.0m, "A mind-bending puzzle with real momentum.", false),
        new("carol@cinelog.dev", "The Dark Knight", 5.0m, "Heath Ledger's Joker is phenomenal.", false),
        new("dave@cinelog.dev", "Pulp Fiction", 4.0m, "Classic Tarantino.", false),
        new("alice@cinelog.dev", "Inception", 5.0m, "Dream within a dream.", false)
    ];

    private static readonly Dictionary<string, int> ExtraReviewCounts = new()
    {
        ["lucy@cinelog.dev"] = 42,
        ["karsten@cinelog.dev"] = 36,
        ["alice@cinelog.dev"] = 31,
        ["emma@cinelog.dev"] = 28,
        ["bob@cinelog.dev"] = 25,
        ["nina@cinelog.dev"] = 23,
        ["carol@cinelog.dev"] = 21,
        ["oscar@cinelog.dev"] = 19,
        ["ivy@cinelog.dev"] = 17,
        ["noah@cinelog.dev"] = 15,
        ["sofia@cinelog.dev"] = 13,
        ["leo@cinelog.dev"] = 11,
        ["mateo@cinelog.dev"] = 9,
        ["tom@cinelog.dev"] = 7,
        ["mila@cinelog.dev"] = 6,
        ["dave@cinelog.dev"] = 5
    };

    private static readonly string[] ReviewTexts =
    [
        "Strong pacing and memorable scenes.",
        "A confident film with a clear point of view.",
        "The performances carry the whole thing.",
        "Not perfect, but very easy to recommend.",
        "Great atmosphere and solid rewatch value.",
        "A few rough edges, but the best moments land.",
        "The direction gives every scene real texture.",
        "Better than expected, especially in the second half.",
        "A clean setup with a satisfying payoff.",
        "The cast makes the material feel sharper.",
        "Visually rich and emotionally direct.",
        "A compact story that never wastes time."
    ];

    internal static async Task SeedAsync(IAppDbContext context)
    {
        var users = await context.Users.ToListAsync();
        var movies = await context.Movies
            .OrderBy(m => m.Title)
            .ToListAsync();

        if (users.Count == 0 || movies.Count == 0)
            return;

        await SeedFixedReviewsAsync(context, users, movies);
        await SeedGeneratedReviewsAsync(context, users, movies);

        await context.SaveChangesAsync();
    }

    private static async Task SeedFixedReviewsAsync(
        IAppDbContext context,
        IReadOnlyList<User> users,
        IReadOnlyList<Movie> movies)
    {
        foreach (var seed in FixedReviews)
        {
            var user = users.FirstOrDefault(u => u.Email == seed.UserEmail);
            var movie = movies.FirstOrDefault(m => m.Title == seed.MovieTitle);

            if (user is null || movie is null)
                continue;

            await AddReviewIfMissingAsync(
                context,
                user.Id,
                movie.Id,
                seed.Rating,
                seed.ReviewText,
                seed.ContainsSpoilers);
        }
    }

    private static async Task SeedGeneratedReviewsAsync(
        IAppDbContext context,
        IReadOnlyList<User> users,
        IReadOnlyList<Movie> movies)
    {
        foreach (var (email, count) in ExtraReviewCounts)
        {
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user is null)
                continue;

            var movieOffset = StableHash(email) % movies.Count;
            var reviewsToCreate = Math.Min(count, movies.Count);

            for (var i = 0; i < reviewsToCreate; i++)
            {
                var movie = movies[(movieOffset + i) % movies.Count];

                await AddReviewIfMissingAsync(
                    context,
                    user.Id,
                    movie.Id,
                    RatingFor(i),
                    ReviewTexts[i % ReviewTexts.Length],
                    containsSpoilers: false);
            }
        }
    }

    private static async Task AddReviewIfMissingAsync(
        IAppDbContext context,
        Guid userId,
        Guid movieId,
        decimal rating,
        string reviewText,
        bool containsSpoilers)
    {
        var exists = await context.Reviews.AnyAsync(r =>
            r.UserId == userId &&
            r.MovieId == movieId);

        if (exists)
            return;

        context.Reviews.Add(Review.Create(
            userId,
            movieId,
            Rating.Create(rating),
            reviewText,
            containsSpoilers));
    }

    private static decimal RatingFor(int index)
    {
        var ratings = new[] { 3.0m, 3.5m, 4.0m, 4.5m, 5.0m };
        return ratings[index % ratings.Length];
    }

    private static int StableHash(string value)
    {
        unchecked
        {
            var hash = 23;

            foreach (var ch in value)
                hash = (hash * 31) + ch;

            return Math.Abs(hash);
        }
    }
}
