using CineLog.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CineLog.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, errors) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found", (object?)null),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict", (object?)null),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", (object?)null),
            ValidationException ve => (
                StatusCodes.Status422UnprocessableEntity,
                "Validation Failed",
                (object)new
                {
                    errors = ve.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray())
                }),
            ValidationDomainException => (StatusCodes.Status422UnprocessableEntity, "Validation Failed", (object?)null),
            DomainException => (StatusCodes.Status400BadRequest, "Bad Request", (object?)null),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", (object?)null)
        };

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode < 500 ? exception.Message : null
        };

        if (errors is not null)
            problemDetails.Extensions["errors"] = errors;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
