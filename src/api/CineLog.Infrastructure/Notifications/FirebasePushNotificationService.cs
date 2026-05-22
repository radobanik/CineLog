using CineLog.Application.Common;
using CineLog.Domain.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CineLog.Infrastructure.Notifications;

public class FirebasePushNotificationService : INotificationService
{
    private readonly IAppDbContext _context;
    private readonly ILogger<FirebasePushNotificationService> _logger;

    public FirebasePushNotificationService(
        IAppDbContext context,
        ILogger<FirebasePushNotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SendAsync(Guid userId, string title, string message, CancellationToken ct = default)
    {
        var token = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.FcmToken)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(token))
            return;

        var fcmMessage = new Message
        {
            Token = token,
            Notification = new Notification { Title = title, Body = message }
        };

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage, ct);
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogWarning(ex, "Firebase push failed for user {UserId}", userId);
        }
    }
}
