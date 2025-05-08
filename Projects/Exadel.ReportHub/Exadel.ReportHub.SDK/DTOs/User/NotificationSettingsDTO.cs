using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.SDK.DTOs.User;

public class NotificationSettingsDTO
{
    public NotificationFrequency Frequency { get; set; }

    public int? DayOfMonth { get; set; }

    public DayOfWeek? DayOfWeek { get; set; }

    public int? Hour { get; set; }

    public ExportFormat ExportFormat { get; set; }

    public DateTime? ReportStartDate { get; set; }

    public DateTime? ReportEndDate { get; set; }
}
