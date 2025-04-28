using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Invoice.Export;

public record InvoiceExportedNotification(Guid UserId, Guid InvoiceId, DateTime TimeStamp, bool IsSuccess) : INotification;
