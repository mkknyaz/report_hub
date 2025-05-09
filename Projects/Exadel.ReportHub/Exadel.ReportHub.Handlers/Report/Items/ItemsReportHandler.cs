using System.Globalization;
using ErrorOr;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Helpers;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Enums;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Items;

public record ItemsReportRequest(Guid ClientId, ExportFormat Format, DateTime? StartDate, DateTime? EndDate) : IRequest<ErrorOr<ExportResult>>;

public class ItemsReportHandler(IItemRepository itemRepository, IInvoiceRepository invoiceRepository, IClientRepository clientRepository, IExportStrategyFactory exportStrategyFactory)
    : IRequestHandler<ItemsReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(ItemsReportRequest request, CancellationToken cancellationToken)
    {
        var exportStrategyTask = exportStrategyFactory.GetStrategyAsync(request.Format, cancellationToken);

        var itemPricesTask = itemRepository.GetClientItemPricesAsync(request.ClientId, cancellationToken);
        var countsTask = invoiceRepository.GetClientItemsCountAsync(request.ClientId, request.StartDate, request.EndDate, cancellationToken);
        var currencyTask = clientRepository.GetCurrencyAsync(request.ClientId, cancellationToken);

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
                $"{ExportFormatHelper.GetFileExtension(request.Format)}",
            ContentType = ExportFormatHelper.GetContentType(request.Format)
        };
    }
}
