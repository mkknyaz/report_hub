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

    public async Task<IEnumerable<TDocument>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Empty;
        return await GetCollection().Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TDocument>> GetAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken)
    {
        return await GetCollection().Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<TDocument> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await GetCollection().Find(filter).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(TDocument entity, CancellationToken cancellationToken)
    {
        await GetCollection().InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Guid id, TDocument entity, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Guid id, UpdateDefinition<TDocument> update, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().DeleteOneAsync(filter, cancellationToken: cancellationToken);
    }

    public IMongoCollection<TDocument> GetCollection() => _context.GetCollection<TDocument>();
}
