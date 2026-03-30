namespace CineLog.Infrastructure.ExternalApis;

/// <summary>Appends the TMDb API key to every outgoing request.</summary>
public class TmdbApiKeyHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public TmdbApiKeyHandler(string apiKey) => _apiKey = apiKey;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uri = request.RequestUri!;
        var separator = string.IsNullOrEmpty(uri.Query) ? "?" : "&";
        request.RequestUri = new Uri($"{uri}{separator}api_key={_apiKey}");
        return base.SendAsync(request, cancellationToken);
    }
}
