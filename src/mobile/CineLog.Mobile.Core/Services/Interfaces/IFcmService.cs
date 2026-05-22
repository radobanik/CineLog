namespace CineLog.Mobile.Core.Services.Interfaces;

public interface IFcmService
{
    Task RegisterTokenAsync(CancellationToken ct = default);
}
