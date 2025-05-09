using Exadel.ReportHub.Csv;
using Exadel.ReportHub.Excel;
using Exadel.ReportHub.Export.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

public static class ExportRegistrations
{
    public static IServiceCollection AddExport(this IServiceCollection services)
    {
        services.AddSingleton<IExportStrategy, CsvExporter>();
        services.AddSingleton<IExportStrategy, ExcelExporter>();
        services.AddSingleton<IExportStrategyFactory, ExportStrategyFactory>();

        return services;
    }
}
