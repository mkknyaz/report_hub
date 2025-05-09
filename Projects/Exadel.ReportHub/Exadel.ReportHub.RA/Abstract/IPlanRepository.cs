using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IPlanRepository
{
    Task<IList<Plan>> GetByClientIdAsync(Guid clientId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    Task<Plan> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Plan plan, CancellationToken cancellationToken);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsForItemByPeriodAsync(Guid itemId, Guid clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateDateAsync(Guid id, Plan plan, CancellationToken cancellationToken);
}
