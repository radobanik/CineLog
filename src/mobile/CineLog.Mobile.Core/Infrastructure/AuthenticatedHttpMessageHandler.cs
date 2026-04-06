using System.Net.Http.Headers;
using CineLog.Mobile.Core.Services.Interfaces;

namespace CineLog.Mobile.Core.Infrastructure;

/// <summary>
/// Injects the JWT Bearer token from ISessionService into every outgoing request.
/// </summary>
public sealed class AuthenticatedHttpMessageHandler : DelegatingHandler
{
    private readonly ISessionService _session;

    public AuthenticatedHttpMessageHandler(ISessionService session)
    {
        _session = session;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_session.Token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.Token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            _session.ClearSession();

        return response;
    }
}
