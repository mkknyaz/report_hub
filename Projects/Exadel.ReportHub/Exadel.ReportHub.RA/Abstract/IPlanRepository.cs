using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IPlanRepository
{
    Task<IList<Plan>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);

    Task<Plan> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Plan plan, CancellationToken cancellationToken);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid itemId, Guid clientId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateDateAsync(Guid id, Plan plan, CancellationToken cancellationToken);
}
