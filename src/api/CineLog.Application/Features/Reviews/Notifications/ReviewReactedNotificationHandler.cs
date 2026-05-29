using CineLog.Application.Common;
using CineLog.Domain.Enums;
using CineLog.Domain.Events;
using CineLog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Application.Features.Reviews.Notifications;

public class ReviewReactedNotificationHandler : INotificationHandler<ReviewReactedEvent>
{
    private readonly IAppDbContext _context;
    private readonly INotificationService _notificationService;

    public ReviewReactedNotificationHandler(IAppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task Handle(ReviewReactedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.ReactionType != ReactionType.Like)
            return;

        var review = await _context.Reviews
            .Select(r => new { r.Id, r.UserId })
            .FirstOrDefaultAsync(r => r.Id == notification.ReviewId, cancellationToken);

        if (review is null || review.UserId == notification.ReactedByUserId)
            return;

        var likerName = await _context.Users
            .Where(u => u.Id == notification.ReactedByUserId)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        await _notificationService.SendAsync(
            review.UserId,
            "New Like",
            $"{likerName} liked your review.",
            cancellationToken);
    }
}
