using CineLog.Application.Features.Users.Watchlist.AddToWatchlist;
using CineLog.Application.Features.Users.Watchlist.CreateWatchlist;
using CineLog.Application.Features.Users.Watchlist.DeleteWatchlist;
using CineLog.Application.Features.Users.Watchlist.GetUserWatchlists;
using CineLog.Application.Features.Users.Watchlist.GetWatchlist;
using CineLog.Application.Features.Users.Watchlist.RemoveFromWatchlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/watchlists")]
public class WatchlistsController : ControllerBase
{
    private readonly ISender _sender;

    public WatchlistsController(ISender sender) => _sender = sender;

    /// <summary>Get all watchlists for the current user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<WatchlistSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<WatchlistSummaryResponse>>> GetAll(CancellationToken ct)
        => Ok(await _sender.Send(new GetUserWatchlistsQuery(), ct));

    /// <summary>Get a watchlist with its movies.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(WatchlistDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WatchlistDetailResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetWatchlistQuery(id), ct));

    /// <summary>Create a new watchlist.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateWatchlistCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>Delete a watchlist.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeleteWatchlistCommand(id), ct);
        return NoContent();
    }

    /// <summary>Add a movie to a watchlist.</summary>
    [HttpPost("{id:guid}/movies/{movieId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddMovie(Guid id, Guid movieId, CancellationToken ct)
    {
        await _sender.Send(new AddToWatchlistCommand(id, movieId), ct);
        return NoContent();
    }

    /// <summary>Remove a movie from a watchlist.</summary>
    [HttpDelete("{id:guid}/movies/{movieId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMovie(Guid id, Guid movieId, CancellationToken ct)
    {
        await _sender.Send(new RemoveFromWatchlistCommand(id, movieId), ct);
        return NoContent();
    }
}
