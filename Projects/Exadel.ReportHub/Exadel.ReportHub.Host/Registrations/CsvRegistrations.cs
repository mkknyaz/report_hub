using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Csv;
using Exadel.ReportHub.Csv.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class CsvRegistrations
{
    public static IServiceCollection AddCsv(this IServiceCollection services)
    {
        services.AddSingleton<ICsvProcessor, CsvProcessor>();

        return services;
    }
}
