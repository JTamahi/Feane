namespace Feane.Models;

public sealed record AuditRecord(
    string CorrelationId,
    string UserName,
    string Path,
    string Method,
    int StatusCode,
    long ElapsedMs,
    DateTimeOffset UtcTime
);
