using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using CineLog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class MovieAndReviewSeeder
{
    private record SeedMovie(int TmdbId, string Title, MovieType Type, string PosterPath, DateOnly ReleaseDate);
    private record SeedReview(string UserEmail, string MovieTitle, decimal Rating, string ReviewText, bool ContainsSpoilers);
    private record SeedFavorite(string UserEmail, string MovieTitle);

    private static readonly SeedMovie[] Movies =
    [
        new(550, "Fight Club", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg", new DateOnly(1999, 10, 15)),
        new(27205, "Inception", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/oYuLEt3zVCKq57qu2F8dT7NIa6f.jpg", new DateOnly(2010, 7, 15)),
        new(155, "The Dark Knight", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/qJ2tW6WMUDux911r6m7haRef0WH.jpg", new DateOnly(2008, 7, 16)),
        new(680, "Pulp Fiction", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/vQWk5YBFWFCPbztNaSRZlsXbgEE.jpg", new DateOnly(1994, 9, 10)),
        new(13, "Forrest Gump", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg", new DateOnly(1994, 7, 6))
    ];

    private static readonly SeedReview[] Reviews =
    [
        new("alice@cinelog.dev", "Fight Club", 4.5m, "First rule is you don't talk about it.", false),
        new("bob@cinelog.dev", "Inception", 4.0m, "A mind-bending masterpiece.", false),
        new("carol@cinelog.dev", "The Dark Knight", 5.0m, "Heath Ledger's Joker is phenomenal.", false),
        new("dave@cinelog.dev", "Pulp Fiction", 4.0m, "Classic Tarantino.", false),
        new("alice@cinelog.dev", "Inception", 5.0m, "Dream within a dream.", false)
    ];

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
        if (users.Count == 0)
            return;

        var movieDict = new Dictionary<string, Movie>();

        foreach (var seed in Movies)
        {
            var movie = await context.Movies.FirstOrDefaultAsync(m => m.IdTmdb == seed.TmdbId);
            if (movie == null)
            {
                movie = Movie.Create(seed.TmdbId, seed.Title, seed.Type);
                movie.UpdateDetails(null, seed.PosterPath, null, seed.ReleaseDate, null);
                context.Movies.Add(movie);
            }
            movieDict[seed.Title] = movie;
        }

        await context.SaveChangesAsync();

        foreach (var seed in Reviews)
        {
            var user = users.FirstOrDefault(u => u.Email == seed.UserEmail);
            var movie = movieDict.GetValueOrDefault(seed.MovieTitle);

            if (user != null && movie != null)
            {
                var existingReview = await context.Reviews.FirstOrDefaultAsync(r => r.UserId == user.Id && r.MovieId == movie.Id);
                if (existingReview == null)
                {
                    var review = Review.Create(user.Id, movie.Id, Rating.Create(seed.Rating), seed.ReviewText, seed.ContainsSpoilers);
                    context.Reviews.Add(review);
                }
            }
        }

        foreach (var seed in Favorites)
        {
            var user = users.FirstOrDefault(u => u.Email == seed.UserEmail);
            var movie = movieDict.GetValueOrDefault(seed.MovieTitle);

            if (user != null && movie != null)
            {
                var existingFav = await context.UserFavorites.FirstOrDefaultAsync(f => f.UserId == user.Id && f.MovieId == movie.Id);
                if (existingFav == null)
                {
                    var favorite = UserFavorite.Create(user.Id, movie.Id);
                    context.UserFavorites.Add(favorite);
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
