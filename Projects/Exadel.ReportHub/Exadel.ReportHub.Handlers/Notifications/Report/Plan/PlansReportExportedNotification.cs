using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Plan;

public record PlansReportExportedNotification(Guid UserId, Guid ClientId, DateTime TimeStamp, bool IsSuccess) : INotification;
