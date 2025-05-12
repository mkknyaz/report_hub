using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;

namespace Exadel.ReportHub.Handlers.Managers.Report;

public interface IReportManager
{
    Task<ExportResult> GenerateInvoicesReportAsync(ExportReportDTO exportReportDto, CancellationToken cancellationToken);

    Task<ExportResult> GenerateItemsReportAsync(ExportReportDTO exportReportDto, CancellationToken cancellationToken);

    Task<ExportResult> GeneratePlansReportAsync(ExportReportDTO exportReportDto, CancellationToken cancellationToken);
}
