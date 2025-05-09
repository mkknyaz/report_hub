using System.Globalization;
using ErrorOr;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Helpers;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Enums;
using MediatR;

namespace Exadel.ReportHub.Handlers.Report.Plans;

public record PlansReportRequest(Guid ClientId, ExportFormat Format, DateTime? StartDate, DateTime? EndDate) : IRequest<ErrorOr<ExportResult>>;

public class PlansReportHandler(IPlanRepository planRepository, IInvoiceRepository invoiceRepository, IItemRepository itemRepository,
    IClientRepository clientRepository, IExportStrategyFactory exportStrategyFactory)
    : IRequestHandler<PlansReportRequest, ErrorOr<ExportResult>>
{
    public async Task<ErrorOr<ExportResult>> Handle(PlansReportRequest request, CancellationToken cancellationToken)
    {
        var exportStrategyTask = exportStrategyFactory.GetStrategyAsync(request.Format, cancellationToken);
        var plansTask = planRepository.GetByClientIdAsync(request.ClientId, request.StartDate, request.EndDate, cancellationToken);
        var reports = new List<PlanReport>();

        await Task.WhenAll(exportStrategyTask, plansTask);
        if (plansTask.Result.Any())
        {
            var countsTask = invoiceRepository.GetPlansActualCountAsync(plansTask.Result, cancellationToken);
            var itemPricesTask = itemRepository.GetClientItemPricesAsync(request.ClientId, cancellationToken);
            var clientCurrencyTask = clientRepository.GetCurrencyAsync(request.ClientId, cancellationToken);

            await Task.WhenAll(countsTask, itemPricesTask, clientCurrencyTask);

            reports = plansTask.Result.Select(x => new PlanReport
            {
                TargetItemId = x.ItemId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                PlannedCount = x.Count,
                ActualCount = countsTask.Result[x.Id],
                Revenue = itemPricesTask.Result[x.ItemId] * countsTask.Result[x.Id],
                ClientCurrency = clientCurrencyTask.Result,
                ReportDate = DateTime.UtcNow
            }).ToList();
        }

        var stream = await exportStrategyTask.Result.ExportAsync(reports, cancellationToken);

        return new ExportResult
        {
            Stream = stream,
            FileName = $"PlansReport_{DateTime.UtcNow.Date.ToString(Export.Abstract.Constants.Format.Date, CultureInfo.InvariantCulture)}" +
                $"{ExportFormatHelper.GetFileExtension(request.Format)}",
            ContentType = ExportFormatHelper.GetContentType(request.Format)
        };
    }
}
