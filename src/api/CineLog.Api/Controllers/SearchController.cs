using CineLog.Application.Common;
using CineLog.Application.Features.Movies;
using CineLog.Application.Features.Movies.SearchMovies;
using CineLog.Application.Features.People;
using CineLog.Application.Features.People.SearchPeople;
using CineLog.Application.Features.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ISender _sender;

    public SearchController(ISender sender) => _sender = sender;

    /// <summary>Search movies and people by a single query.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResponse>> Search(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new SearchQuery(query, page, pageSize), ct));

    /// <summary>Search movies, optionally filtered by genres.</summary>
    [HttpGet("movies")]
    [ProducesResponseType(typeof(PagedResponse<MovieSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<MovieSummaryResponse>>> SearchMovies(
        [FromQuery] string query,
        [FromQuery] List<string>? genres = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new SearchMoviesQuery(query, genres, page, pageSize), ct));

    /// <summary>Search people.</summary>
    [HttpGet("people")]
    [ProducesResponseType(typeof(PagedResponse<PersonSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PersonSummaryResponse>>> SearchPeople(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new SearchPeopleQuery(query, page, pageSize), ct));
}
