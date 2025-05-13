using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class ClientRepository(MongoDbContext context) : BaseRepository(context), IClientRepository
{
    private static readonly FilterDefinitionBuilder<Client> _filterBuilder = Builders<Client>.Filter;

    public Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        return base.AddAsync(client, cancellationToken);
    }

    public Task AddManyAsync(IEnumerable<Client> clients, CancellationToken cancellationToken)
    {
        return base.AddManyAsync(clients, cancellationToken);
    }

    public Task<Client> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync<Client>(id, cancellationToken);
    }

    public Task<IList<Client>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return GetByIdsAsync<Client>(ids, cancellationToken);
    }

    public Task<IList<Client>> GetAsync(CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.IsDeleted, false);

        return GetAsync(filter, cancellationToken);
    }

    public async Task<string> GetCurrencyAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);

        return await GetCollection<Client>().Find(filter).Project(x => x.CurrencyCode).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<string> GetNameAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await GetCollection<Client>().Find(filter).Project(x => x.Name).SingleOrDefaultAsync(cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return SoftDeleteAsync<Client>(id, cancellationToken);
    }

    public Task UpdateNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        var update = Builders<Client>.Update.Set(x => x.Name, name);

        return UpdateAsync(id, update, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExistsAsync<Client>(id, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Name, name);
        var count = await GetCollection<Client>().Find(filter).CountDocumentsAsync(cancellationToken);

        return count > 0;
    }
}
