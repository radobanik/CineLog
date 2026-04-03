using CineLog.Application.Common;
using CineLog.Domain.Events;
using MediatR;

namespace CineLog.Application.Features.Movies.Notifications;

public class MovieElasticDeleteHandler : INotificationHandler<MovieDeletedEvent>
{
    private readonly IElasticSearchService _elasticSearch;

    public MovieElasticDeleteHandler(IElasticSearchService elasticSearch)
        => _elasticSearch = elasticSearch;

    public Task Handle(MovieDeletedEvent notification, CancellationToken cancellationToken)
        => _elasticSearch.DeleteMovieAsync(notification.MovieId, cancellationToken);
}
