using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class ItemRepository(MongoDbContext context) : BaseRepository(context), IItemRepository
{
    private static readonly FilterDefinitionBuilder<Item> _filterBuilder = Builders<Item>.Filter;

    public Task AddAsync(Item item, CancellationToken cancellationToken)
    {
        return base.AddAsync(item, cancellationToken);
    }

    public async Task<bool> AllExistAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.In(x => x.Id, ids);
        var count = await GetCollection<Item>().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count == ids.Count();
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExistsAsync<Item>(id, cancellationToken);
    }

    public Task<IList<Item>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.ClientId, clientId);
        return GetAsync(filter, cancellationToken);
    }

    public Task<Item> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync<Item>(id, cancellationToken);
    }

    public Task<IList<Item>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return GetByIdsAsync<Item>(ids, cancellationToken);
    }

    public async Task<Guid?> GetClientIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await GetCollection<Item>().Find(filter).Project(x => (Guid?)x.ClientId).SingleOrDefaultAsync(cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return SoftDeleteAsync<Item>(id, cancellationToken);
    }

    public Task UpdateAsync(Item item, CancellationToken cancellationToken)
    {
        return UpdateAsync(item.Id, item, cancellationToken);
    }
}
