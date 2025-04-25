using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class CustomerRepository : BaseRepository, ICustomerRepository
{
    private static readonly FilterDefinitionBuilder<Customer> _filterBuilder = Builders<Customer>.Filter;

    public CustomerRepository(MongoDbContext context)
        : base(context)
    {
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        return AddAsync<Customer>(customer, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Email, email);
        var count = await GetCollection<Customer>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExistsAsync<Customer>(id, cancellationToken);
    }

    public Task<IList<Customer>> GetAsync(CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.IsDeleted, false);
        return GetAsync(filter, cancellationToken);
    }

    public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetByIdAsync<Customer>(id, cancellationToken);
    }

    public Task<IList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return GetByIdsAsync<Customer>(ids, cancellationToken);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await SoftDeleteAsync<Customer>(id, cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        var definition = Builders<Customer>.Update
            .Set(x => x.Name, customer.Name)
            .Set(x => x.Country, customer.Country);

        await UpdateAsync<Customer>(customer.Id, definition, cancellationToken);
    }
}
