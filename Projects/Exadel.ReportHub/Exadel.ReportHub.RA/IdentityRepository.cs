using System.Diagnostics.CodeAnalysis;
using Duende.IdentityServer.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class IdentityRepository : BaseRepository, IIdentityRepository
{
    public IdentityRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<TDocument>> GetByNamesAsync<TDocument>(IEnumerable<string> names, CancellationToken cancellationToken)
        where TDocument : Resource
    {
        var filter = Builders<TDocument>.Filter.In(x => x.Name, names);
        return await GetCollection<TDocument>().Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<ApiResource>> GetApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames, CancellationToken cancellationToken)
    {
        var filter = Builders<ApiResource>.Filter.AnyIn(x => x.Scopes, scopeNames);
        return await GetCollection<ApiResource>().Find(filter).ToListAsync();
    }

    public async Task<Client> GetClientByIdAsync(string clientId, CancellationToken cancellationToken)
    {
        var filter = Builders<Client>.Filter.Eq(x => x.ClientId, clientId);
        return await GetCollection<Client>().Find(filter).SingleOrDefaultAsync();
    }
}
