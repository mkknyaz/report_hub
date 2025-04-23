using Exadel.ReportHub.Host.Configs;
using Microsoft.Extensions.Options;

namespace Exadel.ReportHub.Host.Registrations;

public static class PingRegistrations
{
    public static IServiceCollection AddPing(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ReportHubConfig>(configuration.GetSection(nameof(ReportHubConfig)));

        services.AddHttpClient(Constants.HttpClient.PingClient, (cfg, client) =>
        {
            var value = cfg.GetRequiredService<IOptionsMonitor<ReportHubConfig>>().CurrentValue;
            client.BaseAddress = value.Host;
            client.Timeout = value.ConnectionTimeout;
        });

        return services;
    }
}
