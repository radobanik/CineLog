// AUTO-GENERATED — run CineLog.ApiClientGenerator to regenerate full implementation.
namespace CineLog.Mobile.ApiClient.Infrastructure;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string? Response { get; }

    public ApiException(string message, int statusCode, string? response, Exception? innerException)
        : base($"{message}\n\nStatus: {statusCode}\nResponse:\n{response?.Substring(0, Math.Min(512, response.Length))}", innerException)
    {
        StatusCode = statusCode;
        Response   = response;
    }
}

public class ApiException<TResult> : ApiException
{
    public TResult Result { get; }

    public ApiException(string message, int statusCode, string? response, TResult result, Exception? innerException)
        : base(message, statusCode, response, innerException)
    {
        Result = result;
    }
}
