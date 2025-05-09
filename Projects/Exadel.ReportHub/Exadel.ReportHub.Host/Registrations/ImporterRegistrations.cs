using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Csv;
using Exadel.ReportHub.Csv.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class ImporterRegistrations
{
    public static IServiceCollection AddImporters(this IServiceCollection services)
    {
        services.AddSingleton<ICsvImporter, CsvImporter>();

        return services;
    }
}
