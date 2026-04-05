using CineLog.Application.Features.Dashboard.GetNewActions;
using CineLog.Application.Features.Movies;
using CineLog.Application.Features.Movies.GetNewest;
using CineLog.Application.Features.Movies.GetTopRated;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender) => _sender = sender;

    /// <summary>Get top-rated movies.</summary>
    [HttpGet("top-rated-movies")]
    [ProducesResponseType(typeof(List<MovieSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MovieSummaryResponse>>> GetTopRated(
        [FromQuery] int count = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetTopRatedQuery(count), ct));

    /// <summary>Get the latest review activity across all users.</summary>
    [HttpGet("new-actions")]
    [ProducesResponseType(typeof(List<NewActionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<NewActionResponse>>> GetNewActions(
        [FromQuery] int count = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetNewActionsQuery(count), ct));

    /// <summary>Get the newest movies by release date.</summary>
    [HttpGet("newest-movies")]
    [ProducesResponseType(typeof(List<MovieSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MovieSummaryResponse>>> GetNewest(
        [FromQuery] int count = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetNewestQuery(count), ct));
}
