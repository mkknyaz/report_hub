using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Invoice.Export;

public record InvoiceExportedNotification(Guid UserId, Guid InvoiceId, Guid ClientId, DateTime TimeStamp, bool IsSuccess) : INotification;
