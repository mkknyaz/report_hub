using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);

    Task<IList<Customer>> GetAsync(CancellationToken cancellationToken);

    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<Guid> GetClientIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(Customer customer, CancellationToken cancellationToken);
}
