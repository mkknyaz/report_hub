using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Item;

public class AuditItemsReportExportedNotificationHandler(IAuditManager auditManager) : INotificationHandler<ItemsReportExportedNotification>
{
    public async Task Handle(ItemsReportExportedNotification notification, CancellationToken cancellationToken)
    {
        var action = new ExportItemsReportAuditAction(
            userId: notification.UserId,
            clientId: notification.ClientId,
            timeStamp: notification.TimeStamp,
            isSuccess: notification.IsSuccess);

        await auditManager.AuditAsync(action, cancellationToken);
    }
}
