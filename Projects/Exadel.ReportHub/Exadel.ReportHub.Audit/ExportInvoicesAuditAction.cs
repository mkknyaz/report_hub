using Exadel.ReportHub.Audit.Abstract;

namespace Exadel.ReportHub.Audit;

public class ExportInvoicesAuditAction : IAuditAction
{
    public Guid UserId { get; }

    public Guid ClientId { get; }

    public Dictionary<string, Guid> Properties { get; }

    public DateTime TimeStamp { get; }

    public string Action { get; }

    public bool IsSuccess { get; }

    public ExportInvoicesAuditAction(Guid userId, Guid invoiceId, Guid clientId, DateTime timeStamp, bool isSuccess)
    {
        UserId = userId;
        ClientId = clientId;
        Properties = new Dictionary<string, Guid>
        {
            { "InvoiceId", invoiceId }
        };
        TimeStamp = timeStamp;
        Action = nameof(ExportInvoicesAuditAction);
        IsSuccess = isSuccess;
    }
}
