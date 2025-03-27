using Exadel.ReportHub.Data.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
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

    public IMongoCollection<T> GetCollection<T>()
    {
        return _database.GetCollection<T>(typeof(T).Name);
    }
}
