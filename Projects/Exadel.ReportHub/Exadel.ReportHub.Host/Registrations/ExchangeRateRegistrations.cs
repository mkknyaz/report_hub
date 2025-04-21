using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Host.Configs;
using Exadel.ReportHub.Host.Services;
using Exadel.ReportHub.SDK.Abstract;
using Microsoft.Extensions.Options;

namespace Exadel.ReportHub.Host.Registrations;

public static class ExchangeRateRegistrations
{
    public static void AddExchangeRate(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EcbConfig>(configuration.GetSection(nameof(EcbConfig)));

        services.AddHttpClient(Constants.HttpClient.ExchangeRateClient, (cfg, client) =>
        {
            var value = cfg.GetRequiredService<IOptionsMonitor<EcbConfig>>().CurrentValue;
            client.BaseAddress = value.Host;
            client.Timeout = value.ConnectionTimeout;
        });
        services.AddSingleton<IExchangeRateProvider, ExchangeRateProvider>();
        services.AddSingleton<IExchangeRateService, ExchangeRateService>();
    }
}
