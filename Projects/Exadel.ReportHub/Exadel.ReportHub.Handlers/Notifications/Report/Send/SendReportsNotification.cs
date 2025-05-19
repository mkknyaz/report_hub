using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Send;

public record SendReportsNotification(Guid UserId, Guid ClientId, DateTime TimeStamp, bool IsSuccess) : INotification;
