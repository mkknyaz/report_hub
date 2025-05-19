namespace Exadel.ReportHub.Audit.Abstract;

public interface IAuditAction
{
    public Guid UserId { get; }

    public Guid ClientId { get; }

    public Dictionary<string, Guid> Properties { get; }

    public string Action { get; }

    public DateTime TimeStamp { get; }

    public bool IsSuccess { get; }
}
