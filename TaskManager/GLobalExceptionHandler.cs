using Microsoft.AspNetCore.Diagnostics;
using TaskManager.GlobalExceptionHandling;

namespace TaskManager;

public sealed class GLobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GLobalExceptionHandler> _logger;

    public GLobalExceptionHandler(ILogger<GLobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            BaseExceptionHandler e => (e.StatusCode, e.Message),
            _ => (500, $"Sunucu hatası - Tip: {exception.GetType().FullName}")
        };

        if (statusCode == 500)
            _logger.LogError(exception, 
                "Tanimlanmayan hata: {Message}", exception.Message);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(
            new { statusCode, message });

        return true;
    }
}