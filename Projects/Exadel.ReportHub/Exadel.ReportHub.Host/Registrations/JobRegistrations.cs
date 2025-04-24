using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Host.Jobs;
using Exadel.ReportHub.Host.Jobs.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class JobRegistrations
{
    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        var assembly = typeof(PingJob).Assembly;

        var types = assembly.GetTypes().Where(type => typeof(IJob).IsAssignableFrom(type) && !type.IsInterface);

        foreach (var type in types)
        {
            services.AddSingleton(typeof(IJob), type);
        }

        return services;
    }
}
