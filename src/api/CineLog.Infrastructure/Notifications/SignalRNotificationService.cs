using CineLog.Application.Common;
using Microsoft.AspNetCore.SignalR;

namespace CineLog.Infrastructure.Notifications;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendAsync(Guid userId, string title, string message, CancellationToken ct = default)
    {
        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync("ReceiveNotification", new { title, message }, ct);
    }
}
