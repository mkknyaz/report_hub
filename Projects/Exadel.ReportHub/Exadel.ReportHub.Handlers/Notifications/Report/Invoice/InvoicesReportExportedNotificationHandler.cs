using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using Exadel.ReportHub.Handlers.Notifications.Invoice.Export;
using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Invoice;

public class AuditInvoicesReportExportedNotificationHandler(IAuditManager auditManager) : INotificationHandler<InvoicesReportExportedNotification>
{
    public async Task Handle(InvoicesReportExportedNotification notification, CancellationToken cancellationToken)
    {
        var action = new ExportItemsReportAuditAction(
            userId: notification.UserId,
            clientId: notification.ClientId,
            timeStamp: notification.TimeStamp,
            isSuccess: notification.IsSuccess);

        await auditManager.AuditAsync(action, cancellationToken);
    }
}
