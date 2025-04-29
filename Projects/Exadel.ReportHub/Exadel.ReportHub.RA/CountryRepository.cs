using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.RA;

public class CountryRepository(MongoDbContext context) : BaseRepository(context), ICountryRepository
{
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
}
