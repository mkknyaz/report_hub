using Exadel.ReportHub.Host.Services;
using Exadel.ReportHub.SDK.Abstract;
using Hangfire;
using Hangfire.MemoryStorage;

namespace Exadel.ReportHub.Host.Registrations;

public static class HangfireRegistrations
{
    public static IServiceCollection AddHangfire(this IServiceCollection services)
    {
        services.AddHangfire(config =>
            config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());

        services.AddHangfireServer();

        return services;
    }
}
