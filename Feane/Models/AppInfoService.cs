using Microsoft.Extensions.Options;

namespace Feane.Models;

public sealed class AppInfoService
{
    private readonly AppInfoOptions _opts;

    public AppInfoService(IOptions<AppInfoOptions> opts)
    {
        _opts = opts.Value;
    }

    public string Version => _opts.Version ?? "dev";
    public string BuildDateUtc => _opts.BuildDateUtc ?? "";
}
