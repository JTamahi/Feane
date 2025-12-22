using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Feane.Filters;

public sealed class ResponseHeadersResultFilter : IAsyncResultFilter, IOrderedFilter
{
    public int Order => 1000;

    private readonly IConfiguration _config;
    private readonly ILogger<ResponseHeadersResultFilter> _logger;

    public ResponseHeadersResultFilter(IConfiguration config, ILogger<ResponseHeadersResultFilter> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        _logger.LogInformation("ResF BEFORE (Order={Order})", Order);

        var version = _config["App:Version"] ?? "dev";
        var correlationId = context.HttpContext.TraceIdentifier;

        context.HttpContext.Response.Headers["X-App-Version"] = version;
        context.HttpContext.Response.Headers["X-Correlation-Id"] = correlationId;

        context.HttpContext.Response.OnStarting(() =>
        {
            long elapsedMs = 0;

            if (context.HttpContext.Items.TryGetValue(RequestTimingResourceFilter.StopwatchKey, out var obj) &&
                obj is Stopwatch sw)
            {
                if (sw.IsRunning) sw.Stop();
                elapsedMs = sw.ElapsedMilliseconds;
            }

            context.HttpContext.Response.Headers["X-Elapsed-ms"] = elapsedMs.ToString();
            return Task.CompletedTask;
        });

        await next();

        _logger.LogInformation("ResF AFTER (Order={Order})", Order);
    }
}
