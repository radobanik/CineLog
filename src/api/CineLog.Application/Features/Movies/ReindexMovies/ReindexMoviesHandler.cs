using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Movies.ReindexMovies;

public class ReindexMoviesHandler : IRequestHandler<ReindexMoviesCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IElasticSearchService _elasticSearch;

    public ReindexMoviesHandler(IAppDbContext context, IElasticSearchService elasticSearch)
    {
        _context = context;
        _elasticSearch = elasticSearch;
    }

    public async Task<int> Handle(ReindexMoviesCommand request, CancellationToken cancellationToken)
    {
        var movies = await _context.Movies
            .Include(m => m.Genres)
                .ThenInclude(mg => mg.Genre)
            .ToListAsync(cancellationToken);

        var movieDocs = movies.Select(m => new MovieSearchDocument(
            m.Id.ToString(),
            m.IdTmdb,
            m.Title,
            m.OriginalTitle,
            m.Overview,
            m.Type.ToString(),
            m.Genres.Select(mg => mg.Genre.Name).ToList(),
            m.PosterPath,
            m.ReleaseDate?.ToString("yyyy-MM-dd"),
            m.AverageRating,
            m.RatingsCount)).ToList();

        await _elasticSearch.BulkIndexMoviesAsync(movieDocs, cancellationToken);

        var people = await _context.Persons
            .ToListAsync(cancellationToken);

        var personDocs = people.Select(p => new PersonSearchDocument(
            p.Id.ToString(),
            p.Name,
            p.ProfilePath)).ToList();

        await _elasticSearch.BulkIndexPeopleAsync(personDocs, cancellationToken);

        return movies.Count;
    }
}
