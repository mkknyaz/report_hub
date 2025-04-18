using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class ClientRepository : BaseRepository, IClientRepository
{
    private static readonly FilterDefinitionBuilder<Client> _filterBuilder = Builders<Client>.Filter;

    public ClientRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await base.AddAsync(client, cancellationToken);
    }

    public async Task<Client> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetByIdAsync<Client>(id, cancellationToken);
    }

    public async Task<IList<Client>> GetAsync(CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.IsDeleted, false);

        return await GetAsync(filter, cancellationToken);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await SoftDeleteAsync<Client>(id, cancellationToken);
    }

    public async Task UpdateNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        var update = Builders<Client>.Update.Set(x => x.Name, name);
        await UpdateAsync(id, update, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await ExistsAsync<Client>(id, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Name, name);
        var count = await GetCollection<Client>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }
}
