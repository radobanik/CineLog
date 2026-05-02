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
        new(13, "Forrest Gump", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg", new DateOnly(1994, 7, 6)),
        new(603, "The Matrix", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg", new DateOnly(1999, 3, 30)),
        new(78, "Blade Runner", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/63N9uy8nd9j7Eog2axPQ8lbr3Wj.jpg", new DateOnly(1982, 6, 25)),
        new(11, "Star Wars", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/6FfCtAuVAW8XJjZ7eWeLibRLWTw.jpg", new DateOnly(1977, 5, 25)),
        new(120, "The Lord of the Rings: The Fellowship of the Ring", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/6oom5QYQ2yQTMJIbnvbkBL9cHo6.jpg", new DateOnly(2001, 12, 18)),
        new(121, "The Lord of the Rings: The Two Towers", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/5VTN0pR8gcqV3EPUHHfMGnJYN9L.jpg", new DateOnly(2002, 12, 18)),
        new(122, "The Lord of the Rings: The Return of the King", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/rCzpDGLbOoPwLjy3OAm5NUPOTrC.jpg", new DateOnly(2003, 12, 17)),
        new(129, "Spirited Away", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/39wmItIWsg5sZMyRUHLkWBcuVCM.jpg", new DateOnly(2001, 7, 20)),
        new(419430, "Get Out", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/tFXcEccSQMf3lfhfXKSU9iRBpa3.jpg", new DateOnly(2017, 2, 24)),
        new(530385, "Midsommar", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/7LEI8ulZzO5gy9Ww2NVCrKmHeDZ.jpg", new DateOnly(2019, 7, 3)),
        new(694, "The Shining", MovieType.Movie, "https://image.tmdb.org/t/p/w600_and_h900_bestv2/xazWoLealQwEgqZ89MLZklLZD3k.jpg", new DateOnly(1980, 5, 23))

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
