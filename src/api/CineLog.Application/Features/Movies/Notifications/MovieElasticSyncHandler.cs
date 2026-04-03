using CineLog.Application.Common;
using CineLog.Domain.Events;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Movies.Notifications;

public class MovieElasticSyncHandler : INotificationHandler<MovieSyncEvent>
{
    private readonly IAppDbContext _context;
    private readonly IElasticSearchService _elasticSearch;

    public MovieElasticSyncHandler(IAppDbContext context, IElasticSearchService elasticSearch)
    {
        _context = context;
        _elasticSearch = elasticSearch;
    }

    public async Task Handle(MovieSyncEvent notification, CancellationToken cancellationToken)
    {
        var movie = await _context.Movies
            .Include(m => m.Genres)
                .ThenInclude(mg => mg.Genre)
            .Include(m => m.Cast)
                .ThenInclude(mc => mc.Person)
            .Include(m => m.Crew)
                .ThenInclude(mc => mc.Person)
            .FirstOrDefaultAsync(m => m.Id == notification.MovieId, cancellationToken);

        if (movie is null) return;

        var genreNames = movie.Genres.Select(mg => mg.Genre.Name).ToList();

        var movieDoc = new MovieSearchDocument(
            movie.Id.ToString(),
            movie.IdTmdb,
            movie.Title,
            movie.OriginalTitle,
            movie.Overview,
            movie.Type.ToString(),
            genreNames,
            movie.PosterPath,
            movie.ReleaseDate?.ToString("yyyy-MM-dd"),
            movie.AverageRating,
            movie.RatingsCount);

        await _elasticSearch.IndexMovieAsync(movieDoc, cancellationToken);

        var people = movie.Cast.Select(c => c.Person)
            .Concat(movie.Crew.Select(c => c.Person))
            .DistinctBy(p => p.Id);

        foreach (var person in people)
        {
            var personDoc = new PersonSearchDocument(
                person.Id.ToString(),
                person.Name,
                person.ProfilePath);
            await _elasticSearch.IndexPersonAsync(personDoc, cancellationToken);
        }
    }
}
