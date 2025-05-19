using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using Exadel.ReportHub.Handlers.Notifications.Report.Plan;
using MediatR;

namespace Exadel.ReportHub.Handlers.Notifications.Report.Send;

public class AuditSendReportsNotificationHandler(IAuditManager auditManager) : INotificationHandler<SendReportsNotification>
{
    public async Task Handle(SendReportsNotification notification, CancellationToken cancellationToken)
    {
        var action = new SendReportsAuditAction(
            userId: notification.UserId,
            clientId: notification.ClientId,
            timeStamp: notification.TimeStamp,
            isSuccess: notification.IsSuccess);

        await auditManager.AuditAsync(action, cancellationToken);
    }
}
