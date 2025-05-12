using ErrorOr;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.SDK.DTOs.Report;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Items;

public record ItemsReportRequest(ExportReportDTO ExportReportDto) : IRequest<ErrorOr<ExportResult>>;

public class ItemsReportHandler(IReportManager reportManager) : IRequestHandler<ItemsReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(ItemsReportRequest request, CancellationToken cancellationToken)
    {
        return await reportManager.GenerateItemsReportAsync(request.ExportReportDto, cancellationToken);
    }
}
