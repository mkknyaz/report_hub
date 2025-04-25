namespace Exadel.ReportHub.Audit.Abstract;

public interface IAuditManager
{
    Task AuditAsync(IAuditAction auditAction, CancellationToken cancellationToken);
}
