using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.SDK.DTOs.User;

public class UpdateNotificationSettingsDTO
{
    public NotificationFrequency Frequency { get; set; }

    public int? DayOfMonth { get; set; }

    public DayOfWeek? DayOfWeek { get; set; }

    public int Hour { get; set; }

    public Guid ClientId { get; set; }

    public ExportFormat ExportFormat { get; set; }

    public ReportPeriod ReportPeriod { get; set; }

    public int? DaysCount { get; set; }
}
