using CineLog.Application.Common;
using CineLog.Application.Features.Reviews;
using CineLog.Application.Features.Reviews.CreateReview;
using CineLog.Application.Features.Reviews.DeleteReview;
using CineLog.Application.Features.Reviews.GetMovieReviews;
using CineLog.Application.Features.Reviews.ReactToReview;
using CineLog.Application.Features.Reviews.UpdateReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public ReviewsController(ISender sender) => _sender = sender;

    /// <summary>Create a review for a movie.</summary>
    [HttpPost("api/reviews")]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReviewResponse>> Create(
        [FromBody] CreateReviewCommand command,
        CancellationToken ct)
        => Ok(await _sender.Send(command, ct));

    /// <summary>Update an existing review.</summary>
    [HttpPut("api/reviews/{id:guid}")]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponse>> Update(
        Guid id,
        [FromBody] UpdateReviewCommand command,
        CancellationToken ct)
        => Ok(await _sender.Send(command with { ReviewId = id }, ct));

    /// <summary>Delete a review.</summary>
    [HttpDelete("api/reviews/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeleteReviewCommand(id), ct);
        return NoContent();
    }

    /// <summary>Get reviews for a movie.</summary>
    [HttpGet("api/movies/{movieId:guid}/reviews")]
    [ProducesResponseType(typeof(PagedResponse<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ReviewResponse>>> GetMovieReviews(
        Guid movieId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetMovieReviewsQuery(movieId, page, pageSize), ct));

    /// <summary>React to a review.</summary>
    [HttpPost("api/reviews/{id:guid}/react")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> React(
        Guid id,
        [FromBody] ReactToReviewCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command with { ReviewId = id }, ct);
        return NoContent();
    }
}
