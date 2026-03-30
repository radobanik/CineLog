namespace CineLog.Application.Common;

public interface INotificationService
{
    Task SendAsync(Guid userId, string title, string message, CancellationToken ct = default);
}
