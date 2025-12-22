namespace Feane.Middleware;

public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
            correlationId = Guid.NewGuid().ToString("N");

        // Свяжем идентификатор с запросом (удобно для трассировки)
        context.TraceIdentifier = correlationId;

        // Вернем клиенту (чтобы можно было показать на защите в DevTools -> Network -> Headers)
        context.Response.Headers[HeaderName] = correlationId;

        // Пишем в лог (потом будет удобно сверять с request logging / exception handling)
        _logger.LogInformation("CorrelationId = {CorrelationId}", correlationId);

        await _next(context);
    }
}
