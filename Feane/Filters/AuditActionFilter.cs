using System.Diagnostics;
using Feane.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Feane.Filters;

public sealed class AuditActionFilter : IAsyncActionFilter, IOrderedFilter
{
    public int Order => -1000;

    private readonly ILogger<AuditActionFilter> _logger;
    private readonly IAuditService _audit;
    private readonly IDateTimeProvider _clock;

    public AuditActionFilter(
        ILogger<AuditActionFilter> logger,
        IAuditService audit,
        IDateTimeProvider clock)
    {
        _logger = logger;
        _audit = audit;
        _clock = clock;
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

            await _audit.WriteAsync(new AuditRecord(
                correlationId,
                userName,
                path,
                method,
                statusCode,
                sw.ElapsedMilliseconds,
                _clock.UtcNow));

            _logger.LogWarning(
                "AUDIT FAIL {Method} {Path} Action={Action} Status={Status} TimeMs={TimeMs} User={User} CorrelationId={CorrelationId}",
                method, path, actionName, statusCode, sw.ElapsedMilliseconds, userName, correlationId);

            _logger.LogInformation("AF AFTER (Order={Order})", Order);
            return;
        }

        var okStatusCode = context.HttpContext.Response.StatusCode;

        await _audit.WriteAsync(new AuditRecord(
            correlationId,
            userName,
            path,
            method,
            okStatusCode,
            sw.ElapsedMilliseconds,
            _clock.UtcNow));

        _logger.LogInformation(
            "AUDIT END {Method} {Path} Action={Action} Status={Status} TimeMs={TimeMs} User={User} CorrelationId={CorrelationId}",
            method, path, actionName, okStatusCode, sw.ElapsedMilliseconds, userName, correlationId);

        _logger.LogInformation("AF AFTER (Order={Order})", Order);
    }
}
