using ErrorOr;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.SDK.DTOs.Report;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Plans;

public record PlansReportRequest(ExportReportDTO ExportReportDto) : IRequest<ErrorOr<ExportResult>>;

public class PlansReportHandler(IReportManager reportManager) : IRequestHandler<PlansReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(PlansReportRequest request, CancellationToken cancellationToken)
    {
        return await reportManager.GeneratePlansReportAsync(request.ExportReportDto, cancellationToken);
    }
}
