using System.Text;
using Newtonsoft.Json;

namespace CineLog.Mobile.ApiClient.Clients;

public class NotificationsClient : INotificationsClient
{
    private readonly HttpClient _httpClient;

    public NotificationsClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task RegisterFcmTokenAsync(string token, CancellationToken ct = default)
    {
        var body = JsonConvert.SerializeObject(new { token });
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync("api/notifications/fcm-token", content, ct);
        response.EnsureSuccessStatusCode();
    }
}
