using MediatR;

namespace CineLog.Application.Features.Movies.GetMovieDetail;

public record GetMovieDetailQuery(Guid MovieId) : IRequest<MovieDetailResponse>;
