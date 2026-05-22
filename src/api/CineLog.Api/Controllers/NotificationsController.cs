using CineLog.Application.Features.Notifications.RegisterFcmToken;
using CineLog.Application.Features.Notifications.SendTestNotification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender) => _sender = sender;

    /// <summary>Register or update the FCM token for push notifications.</summary>
    [HttpPut("fcm-token")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegisterFcmToken(
        [FromBody] RegisterFcmTokenCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return NoContent();
    }

    /// <summary>Send a test push notification to the current user.</summary>
    [HttpPost("test")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SendTest(
        [FromBody] SendTestNotificationCommand command,
        CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return NoContent();
    }
}
