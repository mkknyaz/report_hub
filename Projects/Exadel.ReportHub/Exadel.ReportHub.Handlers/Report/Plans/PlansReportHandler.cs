using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.Handlers.Notifications;
using Exadel.ReportHub.Handlers.Notifications.Report.Plan;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.SDK.DTOs.Report;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Plans;

public record PlansReportRequest(ExportReportDTO ExportReportDto) : IRequest<ErrorOr<ExportResult>>;

public class PlansReportHandler(IReportManager reportManager, IUserProvider userProvider, IPublisher publisher) : IRequestHandler<PlansReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(PlansReportRequest request, CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var isSuccess = false;

        try
        {
            var result = await reportManager.GeneratePlansReportAsync(request.ExportReportDto, cancellationToken);
            isSuccess = true;

            return result;
        }
        finally
        {
            var notification = new PlansReportExportedNotification(userId, request.ExportReportDto.ClientId, DateTime.UtcNow, isSuccess);
            await publisher.Publish(notification, cancellationToken);
        }
    }
}
