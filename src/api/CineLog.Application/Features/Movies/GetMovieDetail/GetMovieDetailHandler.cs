using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Movies.GetMovieDetail;

public class GetMovieDetailHandler : IRequestHandler<GetMovieDetailQuery, MovieDetailResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMovieDetailHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<MovieDetailResponse> Handle(GetMovieDetailQuery request, CancellationToken cancellationToken)
    {
        var movieTask = _db.Movies
            .Include(m => m.Genres).ThenInclude(mg => mg.Genre)
            .Include(m => m.Cast).ThenInclude(c => c.Person)
            .Include(m => m.Crew).ThenInclude(c => c.Person)
            .Include(m => m.ProductionCompanies).ThenInclude(p => p.Company)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == request.MovieId, cancellationToken);

        var isFavoriteTask = _db.UserFavorites
            .AsNoTracking()
            .AnyAsync(f => f.UserId == _currentUser.UserId && f.MovieId == request.MovieId, cancellationToken);

        await Task.WhenAll(movieTask, isFavoriteTask);

        var movie = movieTask.Result ?? throw new NotFoundException($"Movie {request.MovieId} not found.");

        return new MovieDetailResponse(
            movie.Id,
            movie.IdTmdb,
            movie.Type,
            movie.Title,
            movie.Overview,
            movie.PosterPath,
            movie.BackdropPath,
            movie.ReleaseDate,
            movie.RuntimeMinutes,
            movie.AverageRating,
            movie.RatingsCount,
            movie.Genres.Select(mg => new GenreResponse(mg.Genre.Id, mg.Genre.Name)).ToList(),
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
                .ToList(),
            isFavoriteTask.Result);
    }
}
