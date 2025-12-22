using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Feane.Filters;

public sealed class ResponseHeadersResultFilter : IAsyncResultFilter
{
    private readonly IConfiguration _config;

    public ResponseHeadersResultFilter(IConfiguration config)
    {
        _config = config;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var version = _config["App:Version"] ?? "dev";
        var correlationId = context.HttpContext.TraceIdentifier;

        // Эти заголовки ставим ДО выполнения результата
        context.HttpContext.Response.Headers["X-App-Version"] = version;
        context.HttpContext.Response.Headers["X-Correlation-Id"] = correlationId;

        var sw = Stopwatch.StartNew();

        // X-Elapsed-ms добавим прямо перед отправкой headers
        context.HttpContext.Response.OnStarting(() =>
        {
            sw.Stop();
            context.HttpContext.Response.Headers["X-Elapsed-ms"] = sw.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });

        await next();
    }
}
