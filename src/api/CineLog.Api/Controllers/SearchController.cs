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
}
