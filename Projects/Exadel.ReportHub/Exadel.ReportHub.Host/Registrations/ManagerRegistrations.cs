using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.Handlers.Managers.Invoice;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class ManagerRegistrations
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddSingleton<IInvoiceManager, InvoiceManager>();
        services.AddSingleton<IAuditManager, AuditManager>();
        services.AddSingleton<ICountryBasedEntityManager, CountryBasedEntityManager>();

        return services;
    }
}
