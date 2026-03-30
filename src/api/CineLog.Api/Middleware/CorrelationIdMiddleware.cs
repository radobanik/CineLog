using Serilog.Context;

namespace CineLog.Api.Middleware;

/// <summary>
/// Attaches a unique ID to every request.
/// If the client sends an X-Correlation-Id header, that ID is reused.
/// Otherwise, a new one is generated.
/// The ID is added to the response headers and to every log line for that request,
/// so you can trace a single request across client and server logs.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Response.Headers[CorrelationIdHeader] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
