// AUTO-GENERATED — run CineLog.ApiClientGenerator to regenerate full implementation.
using System.Text;
using CineLog.Mobile.ApiClient.Infrastructure;
using CineLog.Mobile.ApiClient.Models;
using Newtonsoft.Json;

namespace CineLog.Mobile.ApiClient.Clients;

public class AuthClient : IAuthClient
{
    private readonly HttpClient _httpClient;

    public AuthClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> LoginAsync(LoginCommand body, CancellationToken cancellationToken = default)
    {
        var json    = JsonConvert.SerializeObject(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/auth/login", content, cancellationToken);

        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new ApiException("Login failed", (int)response.StatusCode, responseText, null);

        return JsonConvert.DeserializeObject<AuthResponse>(responseText)!;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterCommand body, CancellationToken cancellationToken = default)
    {
        var json     = JsonConvert.SerializeObject(body);
        var content  = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/auth/register", content, cancellationToken);

        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new ApiException("Register failed", (int)response.StatusCode, responseText, null);

        return JsonConvert.DeserializeObject<AuthResponse>(responseText)!;
    }
}
