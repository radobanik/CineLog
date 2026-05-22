using CineLog.Application.Common;
using MediatR;

namespace CineLog.Application.Features.Notifications.SendTestNotification;

public class SendTestNotificationHandler : IRequestHandler<SendTestNotificationCommand>
{
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUser;

    public SendTestNotificationHandler(
        INotificationService notificationService,
        ICurrentUserService currentUser)
    {
        _notificationService = notificationService;
        _currentUser = currentUser;
    }

    public async Task Handle(SendTestNotificationCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.SendAsync(_currentUser.UserId, request.Title, request.Message, cancellationToken);
    }
}
