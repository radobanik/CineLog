using CineLog.Application.Common;
using CineLog.Application.Features.Movies;
using CineLog.Application.Features.Movies.DeleteMovie;
using CineLog.Application.Features.Movies.GetMovieDetail;
using CineLog.Application.Features.Reviews;
using CineLog.Application.Features.Reviews.GetMovieReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly ISender _sender;

    public MoviesController(ISender sender) => _sender = sender;

    /// <summary>Get movie detail by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MovieDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetMovieDetailQuery(id), ct));

    /// <summary>Get reviews for a movie.</summary>
    [HttpGet("{movieId:guid}/reviews")]
    [ProducesResponseType(typeof(PagedResponse<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ReviewResponse>>> GetReviews(
        Guid movieId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetMovieReviewsQuery(movieId, page, pageSize), ct));

    /// <summary>Delete a movie and all its reviews. Admin only.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeleteMovieCommand(id), ct);
        return NoContent();
    }
}
