namespace Feane.Models;

public interface IAuditService
{
    Task WriteAsync(AuditRecord record, CancellationToken ct = default);
}
