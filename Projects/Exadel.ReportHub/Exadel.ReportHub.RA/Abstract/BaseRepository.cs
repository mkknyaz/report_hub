using Exadel.ReportHub.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA.Abstract;

public abstract class BaseRepository<TDocument>
    where TDocument : IDocument
{
    private readonly MongoDbContext _context;
    private static readonly FilterDefinitionBuilder<TDocument> _filterBuilder = Builders<TDocument>.Filter;

    protected BaseRepository(MongoDbContext context)
    {
        _context = context;
    }

    protected async Task<IEnumerable<TDocument>> GetAllAsync(CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Empty;
        return await GetCollection().Find(filter).ToListAsync();
    }

    protected async Task<TDocument> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await GetCollection().Find(filter).SingleOrDefaultAsync();
    }

    protected async Task AddAsync(TDocument entity, CancellationToken cancellationToken)
    {
        await GetCollection().InsertOneAsync(entity);
    }

    protected async Task UpdateAsync(Guid id, TDocument entity, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().ReplaceOneAsync(filter, entity);
    }

    protected async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        await GetCollection().DeleteOneAsync(filter);
    }

    private IMongoCollection<TDocument> GetCollection() => _context.GetCollection<TDocument>();
}
