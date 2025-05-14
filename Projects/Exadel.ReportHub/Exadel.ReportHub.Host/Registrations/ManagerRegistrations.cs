using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Audit;
using Exadel.ReportHub.Audit.Abstract;
using Exadel.ReportHub.Handlers.Managers.Client;
using Exadel.ReportHub.Handlers.Managers.Customer;
using Exadel.ReportHub.Handlers.Managers.Helpers;
using Exadel.ReportHub.Handlers.Managers.Invoice;
using Exadel.ReportHub.Handlers.Managers.Report;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class ManagerRegistrations
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddSingleton<IInvoiceManager, InvoiceManager>();
        services.AddSingleton<IAuditManager, AuditManager>();
        services.AddSingleton<IReportManager, ReportManager>();
        services.AddSingleton<IClientManager, ClientManager>();
        services.AddSingleton<ICustomerManager, CustomerManager>();
        services.AddSingleton<ICountryDataFiller, CountryDataFiller>();

        return services;
    }
}
