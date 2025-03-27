using Exadel.ReportHub.RA;

namespace Exadel.ReportHub.Host.Registrations;

public static class MongoRegistrations
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();
        return services;
    }
}
