using CineLog.Application.Common;
using CineLog.Domain.Events;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Notifications;

public class ReviewReactedNotificationHandler : INotificationHandler<ReviewReactedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IAppDbContext _context;

    public ReviewReactedNotificationHandler(INotificationService notificationService, IAppDbContext context)
    {
        _notificationService = notificationService;
        _context = context;
    }

    public async Task Handle(ReviewReactedEvent notification, CancellationToken cancellationToken)
    {
        var review = await _context.Reviews
            .FirstOrDefaultAsync(r => r.Id == notification.ReviewId, cancellationToken);

        if (review is null) return;

        var reactingUser = await _context.Users
            .Where(u => u.Id == notification.ReactedByUserId)
            .Select(u => new { Username = u.Username.Value })
            .FirstOrDefaultAsync(cancellationToken);

        var username = reactingUser?.Username ?? "Someone";
        var reactionType = notification.ReactionType.ToString().ToLower();

        await _notificationService.SendAsync(
            review.UserId,
            "New reaction on your review",
            $"{username} reacted with {reactionType} to your review.",
            cancellationToken);
    }
}
