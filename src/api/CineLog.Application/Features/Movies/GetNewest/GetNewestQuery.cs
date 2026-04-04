using MediatR;

namespace CineLog.Application.Features.Movies.GetNewest;

public record GetNewestQuery(int Count = 20) : IRequest<List<MovieSummaryResponse>>;
