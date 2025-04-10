using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface ICustomerRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
