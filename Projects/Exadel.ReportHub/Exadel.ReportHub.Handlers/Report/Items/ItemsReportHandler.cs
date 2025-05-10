using System.Globalization;
using ErrorOr;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Helpers;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Items;

public record ItemsReportRequest(ExportReportDTO ExportReportDto) : IRequest<ErrorOr<ExportResult>>;

public class ItemsReportHandler(IItemRepository itemRepository, IInvoiceRepository invoiceRepository, IClientRepository clientRepository, IExportStrategyFactory exportStrategyFactory)
    : IRequestHandler<ItemsReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(ItemsReportRequest request, CancellationToken cancellationToken)
    {
        var exportStrategyTask = exportStrategyFactory.GetStrategyAsync(request.ExportReportDto.Format, cancellationToken);

        var itemPricesTask = itemRepository.GetClientItemPricesAsync(request.ExportReportDto.ClientId, cancellationToken);
        var countsTask = invoiceRepository.GetClientItemsCountAsync(request.ExportReportDto.ClientId,
            request.ExportReportDto.StartDate, request.ExportReportDto.EndDate, cancellationToken);
        var currencyTask = clientRepository.GetCurrencyAsync(request.ExportReportDto.ClientId, cancellationToken);

        await Task.WhenAll(exportStrategyTask, itemPricesTask, countsTask, currencyTask);
        var report = new ItemsReport
        {
            MostSoldItemId = countsTask.Result.Count > 0 ?
                countsTask.Result.MaxBy(x => x.Value).Key : null,
            AveragePrice = itemPricesTask.Result.Count > 0 ?
                itemPricesTask.Result.Average(x => x.Value) : 0,
            AverageRevenue = countsTask.Result.Count > 0 && itemPricesTask.Result.Count > 0 ?
                itemPricesTask.Result.Select(x => x.Value * countsTask.Result.GetValueOrDefault(x.Key)).Average() : 0,
            ClientCurrency = currencyTask.Result,
            ReportDate = DateTime.UtcNow
        };

        var stream = await exportStrategyTask.Result.ExportAsync(report, cancellationToken);

        return new ExportResult
        {
            Stream = stream,
            FileName = $"ItemsReport_{report.ReportDate.Date.ToString(Export.Abstract.Constants.Format.Date, CultureInfo.InvariantCulture)}" +
                $"{ExportFormatHelper.GetFileExtension(request.ExportReportDto.Format)}",
            ContentType = ExportFormatHelper.GetContentType(request.ExportReportDto.Format)
        };
    }
}
