using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var mongoDbSettings = configuration.GetConnectionString("Mongo");
        var client = new MongoClient(mongoDbSettings);
        _database = client.GetDatabase("ReportHub");
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName = null)
    {
        return _database.GetCollection<T>(collectionName ?? typeof(T).Name);
    }
}
