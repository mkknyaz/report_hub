using Exadel.ReportHub.Audit.Abstract;

namespace Exadel.ReportHub.Audit;

public class ExportInvoicesReportAuditAction(Guid userId, Guid clientId, DateTime timeStamp, bool isSuccess) : IAuditAction
{
    public Guid UserId { get; } = userId;

    public Guid ClientId { get; set; } = clientId;

    public Dictionary<string, Guid> Properties { get; }

    public DateTime TimeStamp { get; } = timeStamp;

    public string Action { get; } = nameof(ExportInvoicesReportAuditAction);

    public bool IsSuccess { get; } = isSuccess;
}
