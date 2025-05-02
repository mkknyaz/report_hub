using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.Ecb.Helpers;
using Exadel.ReportHub.Host.Configs;
using Microsoft.Extensions.Options;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class ExchangeRateRegistrations
{
    public static IServiceCollection AddExchangeRate(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EcbConfig>(configuration.GetSection(nameof(EcbConfig)));

        services.AddHttpClient(Constants.HttpClient.ExchangeRateClient, (cfg, client) =>
        {
            var value = cfg.GetRequiredService<IOptionsMonitor<EcbConfig>>().CurrentValue;
            client.BaseAddress = value.Host;
            client.Timeout = value.ConnectionTimeout;
        });
        services.AddSingleton<IExchangeRateClient, ExchangeRateClient>();
        services.AddSingleton<IExchangeRateProvider, ExchangeRateProvider>();
        services.AddSingleton<ICurrencyConverter, CurrencyConverter>();

        return services;
    }
}
