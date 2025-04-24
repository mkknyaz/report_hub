using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.Ecb.Helpers;
using Exadel.ReportHub.Host.Configs;
using Exadel.ReportHub.Host.Services;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Abstract;
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
        services.AddSingleton<ExchangeRateClient>();
        services.AddSingleton<IExchangeRateClient, ExchangeRateProvider>(provider =>
        {
            var exchangeRateRepository = provider.GetRequiredService<IExchangeRateRepository>();
            var exchangeRateService = provider.GetRequiredService<ExchangeRateClient>();

            return new ExchangeRateProvider(exchangeRateRepository, exchangeRateService);
        });
        services.AddSingleton<IExchangeRateService, ExchangeRateService>();
        services.AddSingleton<ICurrencyConverter, CurrencyConverter>();

        return services;
    }
}
