using System.Diagnostics.CodeAnalysis;
using Duende.IdentityServer.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class IdentityRepository(MongoDbContext context) : BaseRepository(context), IIdentityRepository
{
    public async Task<IList<TDocument>> GetByNamesAsync<TDocument>(IEnumerable<string> names, CancellationToken cancellationToken)
        where TDocument : Resource
    {
        var filter = Builders<TDocument>.Filter.In(x => x.Name, names);
        return await GetCollection<TDocument>().Find(filter).ToListAsync();
    }

    public async Task<IList<ApiResource>> GetApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames, CancellationToken cancellationToken)
    {
        var filter = Builders<ApiResource>.Filter.AnyIn(x => x.Scopes, scopeNames);
        return await GetCollection<ApiResource>().Find(filter).ToListAsync();
    }

    public async Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken)
    {
        var filter = Builders<Client>.Filter.Eq(x => x.ClientId, clientId);
        return await GetCollection<Client>("IdentityClient").Find(filter).SingleOrDefaultAsync();
    }
}
