using Exadel.ReportHub.Handlers.Test;

namespace Exadel.ReportHub.Host.Registrations;

public static class MediatrRegistrations
{
    public static void AddMediatr(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateHandler).Assembly));
    }
}
