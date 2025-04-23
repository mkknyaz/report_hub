using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IItemRepository
{
    Task AddAsync(Item item, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<Item>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);

    Task<Item> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<Item>> GetByIdsAsync(IList<Guid> ids, CancellationToken cancellationToken);

    Task<Guid?> GetClientIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(Item item, CancellationToken cancellationToken);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
}
