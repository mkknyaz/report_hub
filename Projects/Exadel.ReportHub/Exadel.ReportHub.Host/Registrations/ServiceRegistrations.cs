using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Host.Services;
using Exadel.ReportHub.SDK.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class ServiceRegistrations
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IInvoiceService, InvoicesService>();
        services.AddTransient<IReportService, ReportsService>();

        return services;
    }
}
