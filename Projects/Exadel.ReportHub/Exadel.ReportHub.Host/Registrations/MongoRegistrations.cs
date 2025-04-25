using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.RA;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class MongoRegistrations
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();
        ConventionRegistry.Register("EnumStringConvention", new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String)
        }, _ => true);
        ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack
        {
            new IgnoreExtraElementsConvention(true)
        }, _ => true);

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        return services;
    }
}
