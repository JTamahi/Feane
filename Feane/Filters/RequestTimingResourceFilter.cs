using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Feane.Filters;

public sealed class RequestTimingResourceFilter : IAsyncResourceFilter, IOrderedFilter
{
    public static readonly object StopwatchKey = new();

    public int Order => -2000;

    private readonly ILogger<RequestTimingResourceFilter> _logger;

    public RequestTimingResourceFilter(ILogger<RequestTimingResourceFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        _logger.LogInformation("RF BEFORE (Order={Order})", Order);

        var sw = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = sw;

        await next();

        if (sw.IsRunning) sw.Stop();

        _logger.LogInformation("RF AFTER (Order={Order})", Order);
    }
}
