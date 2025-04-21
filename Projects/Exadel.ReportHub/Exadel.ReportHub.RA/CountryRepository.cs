using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;

namespace Exadel.ReportHub.RA;

public class CountryRepository : BaseRepository, ICountryRepository
{
    public CountryRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Country>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await GetAllAsync<Country>(cancellationToken);
    }
}
