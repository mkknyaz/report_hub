using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Csv;
using Exadel.ReportHub.Csv.Abstract;
using Exadel.ReportHub.Excel;
using Exadel.ReportHub.Excel.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class ImporterRegistrations
{
    public static IServiceCollection AddImporters(this IServiceCollection services)
    {
        services.AddSingleton<ICsvImporter, CsvImporter>();
        services.AddSingleton<IExcelImporter, ExcelImporter>();

        return services;
    }
}
