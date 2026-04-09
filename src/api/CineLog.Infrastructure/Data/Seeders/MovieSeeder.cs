using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Data.Seeders;

internal static class MovieSeeder
{
    private record SeedMovie(int TmdbId, string Title, MovieType Type, string PosterPath, DateOnly ReleaseDate);

    private static readonly SeedMovie[] Movies =
    [
        new(550, "Fight Club", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg", new DateOnly(1999, 10, 15)),
        new(27205, "Inception", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/oYuLEt3zVCKq57qu2F8dT7NIa6f.jpg", new DateOnly(2010, 7, 15)),
        new(155, "The Dark Knight", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/qJ2tW6WMUDux911r6m7haRef0WH.jpg", new DateOnly(2008, 7, 16)),
        new(680, "Pulp Fiction", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/vQWk5YBFWFCPbztNaSRZlsXbgEE.jpg", new DateOnly(1994, 9, 10)),
        new(13, "Forrest Gump", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg", new DateOnly(1994, 7, 6))
    ];

    internal static async Task SeedAsync(IAppDbContext context)
    {
        foreach (var seed in Movies)
        {
            var exists = await context.Movies.AnyAsync(m => m.IdTmdb == seed.TmdbId);
            if (exists)
                continue;

            var movie = Movie.Create(seed.TmdbId, seed.Title, seed.Type);
            movie.UpdateDetails(null, seed.PosterPath, null, seed.ReleaseDate, null);
            context.Movies.Add(movie);
        }

        await context.SaveChangesAsync();
    }
}
