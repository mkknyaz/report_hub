using Exadel.ReportHub.Data.Enums;

namespace Exadel.ReportHub.Data.Models;

public class NotificationSettings
{
    public NotificationFrequency Frequency { get; set; }

    public int? DayOfMonth { get; set; }

    public DayOfWeek? DayOfWeek { get; set; }

    public int Hour { get; set; }

    public ExportFormat ExportFormat { get; set; }

    public DateTime? ReportStartDate { get; set; }

    public DateTime? ReportEndDate { get; set; }
}
