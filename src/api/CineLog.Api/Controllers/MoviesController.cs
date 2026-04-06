using CineLog.Application.Common;
using CineLog.Application.Features.Movies;
using CineLog.Application.Features.Movies.CreateMovie;
using CineLog.Application.Features.Movies.DeleteMovie;
using CineLog.Application.Features.Movies.GetMovieDetail;
using CineLog.Application.Features.Movies.SetMovieBackdrop;
using CineLog.Application.Features.Movies.SetMoviePoster;
using CineLog.Application.Features.Movies.UpdateMovie;
using CineLog.Application.Features.Reviews;
using CineLog.Application.Features.Reviews.GetMovieReviews;
using CineLog.Application.Features.Users.Favorites.AddToFavorites;
using CineLog.Application.Features.Users.Favorites.RemoveFromFavorites;
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

    /// <summary>Create a new movie.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateMovieCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>Update a movie.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMovieCommand command, CancellationToken ct)
    {
        await _sender.Send(command with { Id = id }, ct);
        return NoContent();
    }

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

    /// <summary>Add movie to current user's favorites.</summary>
    [HttpPost("{id:guid}/favorites")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddToFavorites(Guid id, CancellationToken ct)
    {
        await _sender.Send(new AddToFavoritesCommand(id), ct);
        return NoContent();
    }

    /// <summary>Remove movie from current user's favorites.</summary>
    [HttpDelete("{id:guid}/favorites")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromFavorites(Guid id, CancellationToken ct)
    {
        await _sender.Send(new RemoveFromFavoritesCommand(id), ct);
        return NoContent();
    }

    /// <summary>Set poster image for a movie.</summary>
    [HttpPut("{id:guid}/poster")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SetMovieImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetMovieImageResponse>> SetPoster(Guid id, IFormFile file, CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        var result = await _sender.Send(new SetMoviePosterCommand(id, stream, file.ContentType, file.FileName), ct);
        return Ok(result);
    }

    /// <summary>Set backdrop image for a movie.</summary>
    [HttpPut("{id:guid}/backdrop")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SetMovieImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SetMovieImageResponse>> SetBackdrop(Guid id, IFormFile file, CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        var result = await _sender.Send(new SetMovieBackdropCommand(id, stream, file.ContentType, file.FileName), ct);
        return Ok(result);
    }

    /// <summary>Delete a movie and all its reviews.</summary>
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
