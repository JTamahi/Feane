using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Feane.Filters;

public sealed class ResponseHeadersResultFilter : IAsyncResultFilter
{
    private readonly IConfiguration _config;

    public ResponseHeadersResultFilter(IConfiguration config) => _config = config;

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var version = _config["App:Version"] ?? "dev";
        var correlationId = context.HttpContext.TraceIdentifier;

        context.HttpContext.Response.Headers["X-App-Version"] = version;
        context.HttpContext.Response.Headers["X-Correlation-Id"] = correlationId;

        context.HttpContext.Response.OnStarting(() =>
        {
            long elapsedMs;

            if (context.HttpContext.Items.TryGetValue(RequestTimingResourceFilter.StopwatchKey, out var obj) &&
                obj is Stopwatch sw)
            {
                if (sw.IsRunning) sw.Stop();
                elapsedMs = sw.ElapsedMilliseconds;     // время с ResourceFilter (раннее)
            }
            else
            {
                elapsedMs = 0; // если вдруг filter не сработал
            }

            context.HttpContext.Response.Headers["X-Elapsed-ms"] = elapsedMs.ToString();
            return Task.CompletedTask;
        });

        await next();
    }
}
