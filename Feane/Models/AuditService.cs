using Microsoft.Extensions.Logging;

namespace Feane.Models;

public sealed class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public Task WriteAsync(AuditRecord record, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "AUDIT {Method} {Path} Status={Status} TimeMs={TimeMs} User={User} CorrelationId={CorrelationId}",
            record.Method, record.Path, record.StatusCode, record.ElapsedMs, record.UserName, record.CorrelationId);

        return Task.CompletedTask;
    }
}
