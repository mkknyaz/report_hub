using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Hangfire.MemoryStorage;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
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
