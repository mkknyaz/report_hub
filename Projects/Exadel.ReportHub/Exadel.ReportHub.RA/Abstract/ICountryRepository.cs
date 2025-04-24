using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface ICountryRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<Country>> GetAllAsync(CancellationToken cancellationToken);

    Task<Country> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
