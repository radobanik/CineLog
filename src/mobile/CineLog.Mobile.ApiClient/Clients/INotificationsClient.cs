namespace CineLog.Mobile.ApiClient.Clients;

public interface INotificationsClient
{
    Task RegisterFcmTokenAsync(string token, CancellationToken ct = default);
}
