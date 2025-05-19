using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.Handlers.Notifications.Report.Item;
using Exadel.ReportHub.SDK.DTOs.Report;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Items;

public record ItemsReportRequest(ExportReportDTO ExportReportDto) : IRequest<ErrorOr<ExportResult>>;

public class ItemsReportHandler(IReportManager reportManager, IUserProvider userProvider, IPublisher publisher) : IRequestHandler<ItemsReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(ItemsReportRequest request, CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var isSuccess = false;

        try
        {
            var result = await reportManager.GenerateItemsReportAsync(request.ExportReportDto, cancellationToken);
            isSuccess = true;

            return result;
        }
        finally
        {
            var notification = new ItemsReportExportedNotification(userId, request.ExportReportDto.ClientId, DateTime.UtcNow, isSuccess);
            await publisher.Publish(notification, cancellationToken);
        }
    }
}
