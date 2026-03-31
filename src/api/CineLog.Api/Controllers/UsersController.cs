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
using CineLog.Domain.Repositories;
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
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public UsersController(ISender sender, IUserRepository userRepository, ICurrentUserService currentUser)
    {
        _sender = sender;
        _userRepository = userRepository;
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

    /// <summary>Get user profile by username.</summary>
    [HttpGet("{username}")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetByUsername(
        string username,
        CancellationToken ct)
    {
        var user = await _userRepository.GetByUsernameAsync(username, ct);
        if (user is null) return NotFound();
        return Ok(await _sender.Send(new GetProfileQuery(user.Id), ct));
    }

    /// <summary>Get a user's reviews.</summary>
    [HttpGet("{username}/reviews")]
    [ProducesResponseType(typeof(PagedResponse<ReviewResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<ReviewResponse>>> GetReviews(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, ct);
        if (user is null) return NotFound();
        return Ok(await _sender.Send(new GetUserReviewsQuery(user.Id, page, pageSize), ct));
    }

    /// <summary>Get a user's followers.</summary>
    [HttpGet("{username}/followers")]
    [ProducesResponseType(typeof(PagedResponse<UserSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<UserSummaryResponse>>> GetFollowers(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, ct);
        if (user is null) return NotFound();
        return Ok(await _sender.Send(new GetUserFollowersQuery(user.Id, page, pageSize), ct));
    }

    /// <summary>Get users that a user is following.</summary>
    [HttpGet("{username}/following")]
    [ProducesResponseType(typeof(PagedResponse<UserSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<UserSummaryResponse>>> GetFollowing(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, ct);
        if (user is null) return NotFound();
        return Ok(await _sender.Send(new GetUserFollowingQuery(user.Id, page, pageSize), ct));
    }

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
