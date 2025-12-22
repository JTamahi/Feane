using System.Diagnostics;

namespace Feane.Middleware;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        await _next(context);

        sw.Stop();

        var correlationId = context.TraceIdentifier; // уже проставили в CorrelationIdMiddleware
        var user = context.User?.Identity?.IsAuthenticated == true
            ? context.User.Identity!.Name
            : "anonymous";

        _logger.LogInformation(
            "HTTP {Method} {Path}{Query} -> {StatusCode} in {ElapsedMs} ms (User={User}, CorrelationId={CorrelationId})",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "",
            context.Response.StatusCode,
            sw.ElapsedMilliseconds,
            user,
            correlationId
        );
    }
}
