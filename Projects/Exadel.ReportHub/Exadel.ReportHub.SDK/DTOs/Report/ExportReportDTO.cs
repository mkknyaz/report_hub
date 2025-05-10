using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.SDK.DTOs.Report;

public class ExportReportDTO
{
    public Guid ClientId { get; set; }

    public ExportFormat Format { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
