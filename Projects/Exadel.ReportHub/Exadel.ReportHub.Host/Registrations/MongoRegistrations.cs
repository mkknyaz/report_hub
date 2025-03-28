using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.Host.Registrations;

public static class MongoRegistrations
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<IUserRepository, UserRepository>();
        return services;
    }
}
