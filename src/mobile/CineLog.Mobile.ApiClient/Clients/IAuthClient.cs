// AUTO-GENERATED — run CineLog.ApiClientGenerator to regenerate full implementation.
using CineLog.Mobile.ApiClient.Models;

namespace CineLog.Mobile.ApiClient.Clients;

public interface IAuthClient
{
    Task<AuthResponse> LoginAsync(LoginCommand body, CancellationToken cancellationToken = default);
    Task<AuthResponse> RegisterAsync(RegisterCommand body, CancellationToken cancellationToken = default);
}
