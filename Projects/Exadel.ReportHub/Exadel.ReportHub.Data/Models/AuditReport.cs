using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Data.Models;

public class AuditReport : IDocument
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Dictionary<string, Guid> Properties { get; set; }

    public DateTime TimeStamp { get; set; }

    public string Action { get; set; }

    public bool IsSuccess { get; set; }
}
