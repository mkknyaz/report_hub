using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

public class CountryRepository(MongoDbContext context) : BaseRepository(context), ICountryRepository
{
    private static readonly FilterDefinitionBuilder<Country> _filterBuilder = Builders<Country>.Filter;

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExistsAsync<Country>(id, cancellationToken);
    }

    public Task<IList<Country>> GetAllAsync(CancellationToken cancellationToken)
    {
        return GetAllAsync<Country>(cancellationToken);
    }

    public Task<Country> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync<Country>(id, cancellationToken);
    }

    public async Task<bool> CountryCodeExistsAsync(string countryCode, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.CountryCode, countryCode);
        var count = await GetCollection<Country>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }
}
