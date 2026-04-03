using MediatR;

namespace CineLog.Application.Features.Movies.ReindexMovies;

public record ReindexMoviesCommand : IRequest<int>;
