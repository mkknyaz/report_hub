using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

public class CurrencyRepository : BaseRepository, ICurrencyRepository
{
    private static readonly FilterDefinitionBuilder<Currency> _filterBuilder = Builders<Currency>.Filter;

    public CurrencyRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await ExistsAsync<Currency>(id, cancellationToken);
    }

    public async Task<string> GetCodeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await GetCollection<Currency>().Find(filter).Project(x => x.CurrencyCode).SingleOrDefaultAsync(cancellationToken);
    }
}
