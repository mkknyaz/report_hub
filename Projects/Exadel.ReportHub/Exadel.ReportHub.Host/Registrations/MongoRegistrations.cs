using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Exadel.ReportHub.Host.Registrations;

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

        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserAssignmentRepository, UserAssignmentRepository>();
        services.AddSingleton<IIdentityRepository, IdentityRepository>();
        services.AddSingleton<IClientRepository, ClientRepository>();
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        return services;
    }
}
