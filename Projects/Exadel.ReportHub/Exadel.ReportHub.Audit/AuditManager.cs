using AutoMapper;
using Exadel.ReportHub.Audit.Abstract;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.Audit;

public class AuditManager(IAuditReportRepository auditReportRepository, IMapper mapper) : IAuditManager
{
    public Task AuditAsync(IAuditAction auditAction, CancellationToken cancellationToken)
    {
        var auditReport = mapper.Map<AuditReport>(auditAction);
        return auditReportRepository.AddAsync(auditReport, cancellationToken);
    }
}
