using Exadel.ReportHub.Host.Services;
using Exadel.ReportHub.SDK.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

public static class SchedulerRegistrations
{
    public static IServiceCollection AddScheduler(this IServiceCollection services)
    {
        services.AddSingleton<ISchedulerService, SchedulerService>();

        return services;
    }
}
