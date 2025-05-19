using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.Handlers.Notifications.Report.Invoice;
using Exadel.ReportHub.SDK.DTOs.Report;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Invoices;

public record InvoicesReportRequest(ExportReportDTO ExportReportDto) : IRequest<ErrorOr<ExportResult>>;

public class InvoicesReportHandler(IReportManager reportManager, IUserProvider userProvider, IPublisher publisher) : IRequestHandler<InvoicesReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(InvoicesReportRequest request, CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var isSuccess = false;

        try
        {
            var result = await reportManager.GenerateInvoicesReportAsync(request.ExportReportDto, cancellationToken);
            isSuccess = true;

            return result;
        }
        finally
        {
            var notification = new InvoicesReportExportedNotification(userId, request.ExportReportDto.ClientId, DateTime.UtcNow, isSuccess);
            await publisher.Publish(notification, cancellationToken);
        }
    }
}
