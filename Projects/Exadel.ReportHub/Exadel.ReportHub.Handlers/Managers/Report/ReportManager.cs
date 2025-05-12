using System.Globalization;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Helpers;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;

namespace Exadel.ReportHub.Handlers.Managers.Report;

public class ReportManager(IInvoiceRepository invoiceRepository, IItemRepository itemRepository, IPlanRepository planRepository,
    IClientRepository clientRepository, IExportStrategyFactory exportStrategyFactory) : IReportManager
{
    public async Task<ExportResult> GenerateInvoicesReportAsync(ExportReportDTO exportReportDto, CancellationToken cancellationToken)
    {
        var exportStrategyTask = exportStrategyFactory.GetStrategyAsync(exportReportDto.Format, cancellationToken);
        var reportTask = invoiceRepository.GetReportAsync(exportReportDto.ClientId,
            exportReportDto.StartDate, exportReportDto.EndDate, cancellationToken);
        var clientCurrencyTask = clientRepository.GetCurrencyAsync(exportReportDto.ClientId, cancellationToken);

        await Task.WhenAll(exportStrategyTask, reportTask, clientCurrencyTask);

        var report = reportTask.Result ?? new Data.Models.InvoicesReport();
        report.ClientCurrency = clientCurrencyTask.Result;
        report.ReportDate = DateTime.UtcNow;

        var stream = await exportStrategyTask.Result.ExportAsync(report, cancellationToken);

        return new ExportResult
        {
            Stream = stream,
            FileName = $"InvoicesReport_{DateTime.Today.ToString(Export.Abstract.Constants.Format.Date, CultureInfo.InvariantCulture)}" +
                       $"{ExportFormatHelper.GetFileExtension(exportReportDto.Format)}",
            ContentType = ExportFormatHelper.GetContentType(exportReportDto.Format)
        };
    }

    public async Task<ExportResult> GenerateItemsReportAsync(ExportReportDTO exportReportDto, CancellationToken cancellationToken)
    {
        var exportStrategyTask = exportStrategyFactory.GetStrategyAsync(exportReportDto.Format, cancellationToken);

        var itemPricesTask = itemRepository.GetClientItemPricesAsync(exportReportDto.ClientId, cancellationToken);
        var countsTask = invoiceRepository.GetClientItemsCountAsync(exportReportDto.ClientId,
            exportReportDto.StartDate, exportReportDto.EndDate, cancellationToken);
        var currencyTask = clientRepository.GetCurrencyAsync(exportReportDto.ClientId, cancellationToken);

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
            FileName = $"ItemsReport_{DateTime.Today.ToString(Export.Abstract.Constants.Format.Date, CultureInfo.InvariantCulture)}" +
                       $"{ExportFormatHelper.GetFileExtension(exportReportDto.Format)}",
            ContentType = ExportFormatHelper.GetContentType(exportReportDto.Format)
        };
    }

    public async Task<ExportResult> GeneratePlansReportAsync(ExportReportDTO exportReportDto, CancellationToken cancellationToken)
    {
        var exportStrategyTask = exportStrategyFactory.GetStrategyAsync(exportReportDto.Format, cancellationToken);
        var plansTask = planRepository.GetByClientIdAsync(exportReportDto.ClientId,
            exportReportDto.StartDate, exportReportDto.EndDate, cancellationToken);
        var reports = new List<PlanReport>();

        await Task.WhenAll(exportStrategyTask, plansTask);
        if (plansTask.Result.Any())
        {
            var countsTask = invoiceRepository.GetPlansActualCountAsync(plansTask.Result, cancellationToken);
            var itemPricesTask = itemRepository.GetClientItemPricesAsync(exportReportDto.ClientId, cancellationToken);
            var clientCurrencyTask = clientRepository.GetCurrencyAsync(exportReportDto.ClientId, cancellationToken);

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
            FileName = $"PlansReport_{DateTime.Today.ToString(Export.Abstract.Constants.Format.Date, CultureInfo.InvariantCulture)}" +
                       $"{ExportFormatHelper.GetFileExtension(exportReportDto.Format)}",
            ContentType = ExportFormatHelper.GetContentType(exportReportDto.Format)
        };
    }
}
