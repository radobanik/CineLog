using MediatR;

namespace CineLog.Application.Features.Movies.GetTopRated;

public record GetTopRatedQuery(int Count = 20) : IRequest<List<MovieSummaryResponse>>;
