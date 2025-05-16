using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Plan;

public class AuditPlansReportExportedNotificationHandler(IAuditManager auditManager) : INotificationHandler<PlansReportExportedNotification>
{
    public async Task Handle(PlansReportExportedNotification notification, CancellationToken cancellationToken)
    {
        var action = new ExportPlansReportAuditAction(
            userId: notification.UserId,
            clientId: notification.ClientId,
            timeStamp: notification.TimeStamp,
            isSuccess: notification.IsSuccess);

        await auditManager.AuditAsync(action, cancellationToken);
    }
}
