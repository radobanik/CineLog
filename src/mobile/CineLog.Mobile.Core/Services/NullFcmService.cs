using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Services;

public sealed class NullFcmService : IFcmService
{
    public Task RegisterTokenAsync(CancellationToken ct = default) => Task.CompletedTask;
}
