using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Movies.GetMovieDetail;

public class GetMovieDetailHandler : IRequestHandler<GetMovieDetailQuery, MovieDetailResponse>
{
    private readonly IAppDbContext _db;

    public GetMovieDetailHandler(IAppDbContext db) => _db = db;

    public async Task<MovieDetailResponse> Handle(GetMovieDetailQuery request, CancellationToken cancellationToken)
    {
        var movie = await _db.Movies
            .Include(m => m.Cast).ThenInclude(c => c.Person)
            .Include(m => m.Crew).ThenInclude(c => c.Person)
            .Include(m => m.ProductionCompanies).ThenInclude(p => p.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.MovieId, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        return new MovieDetailResponse(
            movie.Id,
            movie.TmdbId,
            movie.Type,
            movie.Title,
            movie.Overview,
            movie.PosterPath,
            movie.BackdropPath,
            movie.ReleaseDate,
            movie.RuntimeMinutes,
            movie.AverageRating,
            movie.RatingsCount,
            movie.Genres.ToList(),
            movie.ImdbId,
            movie.OriginalTitle,
            movie.OriginalLanguage,
            movie.Tagline,
            movie.Status,
            movie.Budget,
            movie.Revenue,
            movie.Popularity,
            movie.NumberOfSeasons,
            movie.NumberOfEpisodes,
            movie.Cast
                .OrderBy(c => c.Order)
                .Select(c => new CastMemberResponse(c.PersonId, c.Person.Name, c.Character, c.Order, c.Person.ProfilePath))
                .ToList(),
            movie.Crew
                .Select(c => new CrewMemberResponse(c.PersonId, c.Person.Name, c.Department, c.Job, c.Person.ProfilePath))
                .ToList(),
            movie.ProductionCompanies
                .Select(p => new ProductionCompanyResponse(p.Company.Id, p.Company.Name, p.Company.LogoPath, p.Company.OriginCountry))
                .ToList());
    }
}
