using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Feane.Filters;

public sealed class AuditActionFilter : IAsyncActionFilter
{
    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(ILogger<AuditActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var sw = Stopwatch.StartNew();

        var correlationId = context.HttpContext.TraceIdentifier;
        var userName = context.HttpContext.User?.Identity?.Name ?? "anonymous";

        var actionName = context.ActionDescriptor.DisplayName ?? "unknown-action";
        var path = context.HttpContext.Request.Path.ToString();
        var method = context.HttpContext.Request.Method;

        // Важно: не логируем пароли/большие тела запроса. Логируем только имена аргументов.
        var args = string.Join(", ", context.ActionArguments.Keys);

        _logger.LogInformation("AUDIT START {Method} {Path} Action={Action} Args=[{Args}] User={User} CorrelationId={CorrelationId}",
            method, path, actionName, args, userName, correlationId);

        var executed = await next();
        sw.Stop();

        int statusCode;

        if (executed.Exception != null && !executed.ExceptionHandled)
        {
            statusCode = StatusCodes.Status500InternalServerError;

            _logger.LogWarning("AUDIT FAIL {Method} {Path} Action={Action} Status={Status} TimeMs={TimeMs} User={User} CorrelationId={CorrelationId}",
                method, path, actionName, statusCode, sw.ElapsedMilliseconds, userName, correlationId);

            return;
        }

        statusCode = context.HttpContext.Response.StatusCode;

        _logger.LogInformation("AUDIT END {Method} {Path} Action={Action} Status={Status} TimeMs={TimeMs} User={User} CorrelationId={CorrelationId}",
            method, path, actionName, statusCode, sw.ElapsedMilliseconds, userName, correlationId);

    }
}
