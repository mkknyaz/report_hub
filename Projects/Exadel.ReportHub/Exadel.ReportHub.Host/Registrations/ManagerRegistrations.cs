using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Managers;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class ManagerRegistrations
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddSingleton<IInvoiceManager, InvoiceManager>();

        return services;
    }
}
