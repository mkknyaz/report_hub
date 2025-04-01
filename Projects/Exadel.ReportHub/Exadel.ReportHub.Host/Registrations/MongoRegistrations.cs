using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Exadel.ReportHub.Host.Registrations;

public static class MongoRegistrations
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton(typeof(IIdentityRepository<>), typeof(IdentityRepository<>));
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        return services;
    }
}
