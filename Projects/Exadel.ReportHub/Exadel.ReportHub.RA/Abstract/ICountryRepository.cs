using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface ICountryRepository
{
    Task<IList<Country>> GetAllAsync(CancellationToken cancellationToken);
}
