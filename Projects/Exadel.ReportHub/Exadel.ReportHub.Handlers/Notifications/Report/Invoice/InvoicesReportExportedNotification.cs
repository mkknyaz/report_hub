using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Invoice;

public record InvoicesReportExportedNotification(Guid UserId, Guid ClientId, DateTime TimeStamp, bool IsSuccess) : INotification;
