using CineLog.Application.Common;
using CineLog.Application.Features.Reviews;
using CineLog.Application.Features.Users;
using CineLog.Application.Features.Users.FollowUser;
using CineLog.Application.Features.Users.GetProfile;
using CineLog.Application.Features.Users.GetUserFollowers;
using CineLog.Application.Features.Users.GetUserFollowing;
using CineLog.Application.Features.Users.GetUserReviews;
using CineLog.Application.Features.Users.UnfollowUser;
using CineLog.Application.Features.Users.UpdateProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public UsersController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    /// <summary>Get the current user's profile.</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileResponse>> GetMe(CancellationToken ct)
        => Ok(await _sender.Send(new GetProfileQuery(_currentUser.UserId), ct));

    /// <summary>Update the current user's profile.</summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileResponse>> UpdateMe(
        [FromBody] UpdateProfileCommand command,
        CancellationToken ct)
        => Ok(await _sender.Send(command, ct));

    /// <summary>Get user profile by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetProfileQuery(id), ct));

    /// <summary>Get a user's reviews.</summary>
    [HttpGet("{id:guid}/reviews")]
    [ProducesResponseType(typeof(PagedResponse<ReviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<ReviewResponse>>> GetReviews(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetUserReviewsQuery(id, page, pageSize), ct));

    /// <summary>Get a user's followers.</summary>
    [HttpGet("{id:guid}/followers")]
    [ProducesResponseType(typeof(PagedResponse<UserSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<UserSummaryResponse>>> GetFollowers(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetUserFollowersQuery(id, page, pageSize), ct));

    /// <summary>Get users that a user is following.</summary>
    [HttpGet("{id:guid}/following")]
    [ProducesResponseType(typeof(PagedResponse<UserSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<UserSummaryResponse>>> GetFollowing(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await _sender.Send(new GetUserFollowingQuery(id, page, pageSize), ct));

    /// <summary>Follow a user.</summary>
    [HttpPost("{id:guid}/follow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Follow(Guid id, CancellationToken ct)
    {
        await _sender.Send(new FollowUserCommand(id), ct);
        return NoContent();
    }

    /// <summary>Unfollow a user.</summary>
    [HttpDelete("{id:guid}/follow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unfollow(Guid id, CancellationToken ct)
    {
        await _sender.Send(new UnfollowUserCommand(id), ct);
        return NoContent();
    }
}
