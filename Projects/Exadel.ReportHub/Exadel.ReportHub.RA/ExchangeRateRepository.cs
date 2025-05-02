using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class ExchangeRateRepository(MongoDbContext context) : BaseRepository(context), IExchangeRateRepository
{
    private static readonly FilterDefinitionBuilder<ExchangeRate> _filterBuilder = Builders<ExchangeRate>.Filter;

    public async Task UpsertManyAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken)
    {
        var opt = new List<WriteModel<ExchangeRate>>();
        foreach (var exchangeRate in exchangeRates)
        {
            var filter = _filterBuilder.And(
                _filterBuilder.Eq(x => x.Currency, exchangeRate.Currency),
                _filterBuilder.Eq(x => x.RateDate, exchangeRate.RateDate));
            var update = Builders<ExchangeRate>.Update
                .Set(x => x.Rate, exchangeRate.Rate)
                .SetOnInsert(x => x.Id, Guid.NewGuid());

            opt.Add(new UpdateOneModel<ExchangeRate>(filter, update)
            {
                IsUpsert = true
            });
        }

        await GetCollection<ExchangeRate>().BulkWriteAsync(opt, cancellationToken: cancellationToken);
    }

    public async Task<ExchangeRate> GetByCurrencyAsync(string currency, DateTime date, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.Currency, currency),
            _filterBuilder.Eq(x => x.RateDate, date));
        return await GetCollection<ExchangeRate>().Find(filter).SingleOrDefaultAsync(cancellationToken);
    }
}
