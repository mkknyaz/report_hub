using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class ExchangeRateRepository : BaseRepository, IExchangeRateRepository
{
    private static readonly FilterDefinitionBuilder<ExchangeRate> _filterBuilder = Builders<ExchangeRate>.Filter;

    public ExchangeRateRepository(MongoDbContext context)
        : base(context)
    {
    }

    public Task AddManyAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
    {
        return base.AddManyAsync(exchangeRates, cancellationToken);
    }

    public Task<IList<ExchangeRate>> GetAllAsync(CancellationToken cancellationToken)
    {
        return GetAllAsync<ExchangeRate>(cancellationToken);
    }

    public async Task<ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Currency, currency);
        return await GetCollection<ExchangeRate>().Find(filter).SingleOrDefaultAsync(cancellationToken);
    }
}
