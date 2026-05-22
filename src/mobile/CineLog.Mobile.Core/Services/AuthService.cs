using CineLog.Mobile.ApiClient.Clients;
using CineLog.Mobile.ApiClient.Models;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class AuthService : IAuthService
{
    private readonly IAuthClient _authClient;
    private readonly ISessionService _session;
    private readonly IFcmService _fcmService;

    public AuthService(IAuthClient authClient, ISessionService session, IFcmService fcmService)
    {
        _authClient = authClient;
        _session = session;
        _fcmService = fcmService;
    }

    public async Task LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var response = await _authClient.LoginAsync(
            new LoginCommand { Email = email, Password = password }, ct);

        await _session.SetSessionAsync(response.Token!, response.UserId!.Value, response.Username!);
        _ = _fcmService.RegisterTokenAsync(CancellationToken.None);
    }

    public async Task RegisterAsync(string username, string email, string password, CancellationToken ct = default)
    {
        var response = await _authClient.RegisterAsync(
            new RegisterCommand { Username = username, Email = email, Password = password }, ct);

        await _session.SetSessionAsync(response.Token!, response.UserId!.Value, response.Username!);
        _ = _fcmService.RegisterTokenAsync(CancellationToken.None);
    }

    public Task LogoutAsync()
    {
        _session.ClearSession();
        return Task.CompletedTask;
    }
}
