using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IAuditReportRepository
{
    Task AddAsync(AuditReport auditReport, CancellationToken cancellationToken);

    Task<AuditReport> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<AuditReport>> GetByUserIdAsync(Guid userId, int skip, int top, CancellationToken cancellationToken);

    Task<long> GetCountAsync(Guid userId, CancellationToken cancellationToken);
}
