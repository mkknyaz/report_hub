using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Invoice.Export;

public class AuditInvoiceExportedNotificationHandler(IAuditManager auditManager) : INotificationHandler<InvoiceExportedNotification>
{
    public async Task Handle(InvoiceExportedNotification notification, CancellationToken cancellationToken)
    {
        var action = new ExportInvoicesAuditAction(
            userId: notification.UserId,
            invoiceId: notification.InvoiceId,
            timeStamp: notification.TimeStamp,
            isSuccess: notification.IsSuccess);

        await auditManager.AuditAsync(action, cancellationToken);
    }
}
