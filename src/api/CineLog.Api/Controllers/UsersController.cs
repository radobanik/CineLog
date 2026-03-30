using CineLog.Application.Features.Users;
using CineLog.Application.Features.Users.FollowUser;
using CineLog.Application.Features.Users.GetProfile;
using CineLog.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IUserRepository _userRepository;

    public UsersController(ISender sender, IUserRepository userRepository)
    {
        _sender = sender;
        _userRepository = userRepository;
    }

    /// <summary>Get user profile by username.</summary>
    [HttpGet("{username}")]
    [Authorize]
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

    /// <summary>Follow a user.</summary>
    [HttpPost("{id:guid}/follow")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Follow(Guid id, CancellationToken ct)
    {
        await _sender.Send(new FollowUserCommand(id), ct);
        return NoContent();
    }
}
