using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class RepositoryRegistrations
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserAssignmentRepository, UserAssignmentRepository>();
        services.AddSingleton<IIdentityRepository, IdentityRepository>();
        services.AddSingleton<IClientRepository, ClientRepository>();
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
        services.AddSingleton<IExchangeRateRepository, ExchangeRateRepository>();
        services.AddSingleton<IItemRepository, ItemRepository>();
        services.AddSingleton<ICurrencyRepository, CurrencyRepository>();
        services.AddSingleton<IPlanRepository, PlanRepository>();
        services.AddSingleton<ICountryRepository, CountryRepository>();

        return services;
    }
}
