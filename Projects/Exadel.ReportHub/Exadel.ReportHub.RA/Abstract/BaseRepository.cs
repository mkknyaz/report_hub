using Exadel.ReportHub.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA.Abstract;

public abstract class BaseRepository<TDocument>
    where TDocument : IDocument
{
    protected static readonly FilterDefinitionBuilder<TDocument> _filterBuilder = Builders<TDocument>.Filter;
    private readonly MongoDbContext _context;

    protected BaseRepository(MongoDbContext context)
    {
        _context = context;
    }

    protected async Task<IEnumerable<TDocument>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Empty;
        return await GetCollection().Find(filter).ToListAsync(cancellationToken);
    }

    protected async Task<IEnumerable<TDocument>> GetAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken)
    {
        return await GetCollection().Find(filter).ToListAsync(cancellationToken);
    }

    protected async Task<TDocument> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await GetCollection().Find(filter).SingleOrDefaultAsync(cancellationToken);
    }

    protected async Task AddAsync(TDocument entity, CancellationToken cancellationToken)
    {
        await GetCollection().InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    protected async Task UpdateAsync(Guid id, TDocument entity, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    protected async Task UpdateAsync(Guid id, UpdateDefinition<TDocument> update, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    protected async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().DeleteOneAsync(filter, cancellationToken: cancellationToken);
    }

    protected IMongoCollection<TDocument> GetCollection() => _context.GetCollection<TDocument>();
}
