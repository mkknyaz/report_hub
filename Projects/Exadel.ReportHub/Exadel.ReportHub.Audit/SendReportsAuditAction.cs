using Exadel.ReportHub.Audit.Abstract;

namespace Exadel.ReportHub.Audit;

public class SendReportsAuditAction(Guid userId, Guid clientId, DateTime timeStamp, bool isSuccess) : IAuditAction
{
    public Guid UserId { get; } = userId;

    public Guid ClientId { get; } = clientId;

    public Dictionary<string, Guid> Properties { get; }

    public DateTime TimeStamp { get; } = timeStamp;

    public string Action { get; } = nameof(SendReportsAuditAction);

    public bool IsSuccess { get; } = isSuccess;
}
