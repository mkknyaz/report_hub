using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Item;

public record ItemsReportExportedNotification(Guid UserId, Guid ClientId, DateTime TimeStamp, bool IsSuccess) : INotification;
