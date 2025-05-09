using CsvHelper.Configuration;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Csv.ClassMaps;

public static class ClassMapFactory
{
    public static ClassMap GetClassMap<TModel>()
    {
        return typeof(TModel).Name switch
        {
            nameof(InvoicesReport) => new InvoicesReportMap(),
            nameof(ItemsReport) => new ItemsReportMap(),
            nameof(PlanReport) => new PlansReportMap(),
            nameof(CreateInvoiceDTO) => new CreateInvoiceMap(),
            _ => throw new ArgumentException($"No ClassMap for {typeof(TModel).Name}")
        };
    }
}
