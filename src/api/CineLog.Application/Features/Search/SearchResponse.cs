using CineLog.Application.Features.Movies;
using CineLog.Application.Features.People;

namespace CineLog.Application.Features.Search;

public record SearchResponse(
    List<MovieSummaryResponse> Movies,
    int TotalMovies,
    List<PersonSummaryResponse> People,
    int TotalPeople);
