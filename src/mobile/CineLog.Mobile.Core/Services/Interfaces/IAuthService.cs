namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IAuthService
{
    Task LoginAsync(string email, string password, CancellationToken ct = default);
    Task RegisterAsync(string username, string email, string password, CancellationToken ct = default);
    Task LogoutAsync();
}
