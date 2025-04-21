using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> GetAllAsync(CancellationToken cancellationToken);
}
