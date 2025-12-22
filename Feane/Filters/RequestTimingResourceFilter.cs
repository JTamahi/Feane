using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Feane.Filters;

public sealed class RequestTimingResourceFilter : IAsyncResourceFilter
{
    // ключ для HttpContext.Items (чтобы не писать строкой)
    public static readonly object StopwatchKey = new();

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        var sw = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = sw;

        await next();

        // стопать тут необязательно (можно в ResultFilter),
        // но можно оставить как "cleanup"
        if (context.HttpContext.Items[StopwatchKey] is Stopwatch s && s.IsRunning)
            s.Stop();
    }
}
