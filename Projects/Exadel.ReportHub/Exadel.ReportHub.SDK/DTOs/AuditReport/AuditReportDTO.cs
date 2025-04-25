using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.SDK.DTOs.AuditReport;

public class AuditReportDTO
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Dictionary<string, Guid> Properties { get; set; }

    public string Action { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool IsSuccess { get; set; }
}
