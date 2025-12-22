using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Feane.Filters;

public sealed class AuditActionFilter : IAsyncActionFilter, IOrderedFilter
{
    public int Order => -1000;

    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(ILogger<AuditActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _logger.LogInformation("AF BEFORE (Order={Order})", Order);

        var sw = Stopwatch.StartNew();

        var correlationId = context.HttpContext.TraceIdentifier;
        var userName = context.HttpContext.User?.Identity?.Name ?? "anonymous";

        var actionName = context.ActionDescriptor.DisplayName ?? "unknown-action";
        var path = context.HttpContext.Request.Path.ToString();
        var method = context.HttpContext.Request.Method;

        var args = string.Join(", ", context.ActionArguments.Keys);

        _logger.LogInformation(
            "AUDIT START {Method} {Path} Action={Action} Args=[{Args}] User={User} CorrelationId={CorrelationId}",
            method, path, actionName, args, userName, correlationId);

        var executed = await next();

        sw.Stop();

        if (executed.Exception != null && !executed.ExceptionHandled)
        {
            var statusCode = StatusCodes.Status500InternalServerError;

            _logger.LogWarning(
                "AUDIT FAIL {Method} {Path} Action={Action} Status={Status} TimeMs={TimeMs} User={User} CorrelationId={CorrelationId}",
                method, path, actionName, statusCode, sw.ElapsedMilliseconds, userName, correlationId);

            // Не выставляем ExceptionHandled=true — пусть глобальный handler обработает.
            _logger.LogInformation("AF AFTER (Order={Order})", Order);
            return;
        }

        var okStatusCode = context.HttpContext.Response.StatusCode;

        _logger.LogInformation(
            "AUDIT END {Method} {Path} Action={Action} Status={Status} TimeMs={TimeMs} User={User} CorrelationId={CorrelationId}",
            method, path, actionName, okStatusCode, sw.ElapsedMilliseconds, userName, correlationId);

        _logger.LogInformation("AF AFTER (Order={Order})", Order);
    }
}
