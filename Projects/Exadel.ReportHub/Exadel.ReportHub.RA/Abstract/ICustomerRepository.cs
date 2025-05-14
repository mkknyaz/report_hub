using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface ICustomerRepository
{
    Task AddManyAsync(IEnumerable<Customer> customers, CancellationToken cancellationToken);

    Task AddAsync(Customer customer, CancellationToken cancellationToken);

    Task<IList<Customer>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);

    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<bool> ExistsOnClientAsync(Guid id, Guid clientId, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(Customer customer, CancellationToken cancellationToken);
}
