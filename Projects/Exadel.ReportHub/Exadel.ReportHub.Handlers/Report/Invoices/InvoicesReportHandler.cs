using System.Globalization;
using ErrorOr;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Helpers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Enums;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Invoices;

public record InvoicesReportRequest(Guid ClientId, ExportFormat Format, DateTime? StartDate, DateTime? EndDate) : IRequest<ErrorOr<ExportResult>>;

public class InvoicesReportHandler(IInvoiceRepository invoiceRepository, IClientRepository clientRepository, IExportStrategyFactory exportStrategyFactory)
    : IRequestHandler<InvoicesReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(InvoicesReportRequest request, CancellationToken cancellationToken)
    {
        var exportStrategyTask = exportStrategyFactory.GetStrategyAsync(request.Format, cancellationToken);
        var reportTask = invoiceRepository.GetReportAsync(request.ClientId, request.StartDate, request.EndDate, cancellationToken);
        var clientCurrencyTask = clientRepository.GetCurrencyAsync(request.ClientId, cancellationToken);

        await Task.WhenAll(exportStrategyTask, reportTask, clientCurrencyTask);

        var report = reportTask.Result ?? new Data.Models.InvoicesReport();
        report.ClientCurrency = clientCurrencyTask.Result;
        report.ReportDate = DateTime.UtcNow;

        var stream = await exportStrategyTask.Result.ExportAsync(report, cancellationToken);

        return new ExportResult
        {
            Stream = stream,
            FileName = $"InvoicesReport_{report.ReportDate.Date.ToString(Export.Abstract.Constants.Format.Date, CultureInfo.InvariantCulture)}" +
                $"{ExportFormatHelper.GetFileExtension(request.Format)}",
            ContentType = ExportFormatHelper.GetContentType(request.Format)
        };
    }
}
