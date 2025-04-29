using System.Diagnostics.CodeAnalysis;
using Duende.IdentityServer.Models;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class CustomerRepository(MongoDbContext context) : BaseRepository(context), ICustomerRepository
{
    private static readonly FilterDefinitionBuilder<Customer> _filterBuilder = Builders<Customer>.Filter;

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

    public async Task<bool> ExistsAsync(Guid id, Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.Id, id),
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Eq(x => x.IsDeleted, false));
        var count = await GetCollection<Customer>().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public Task<IList<Customer>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Eq(x => x.IsDeleted, false));
        return GetAsync(filter, cancellationToken);
    }

    public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetByIdAsync<Customer>(id, cancellationToken);
    }

    public async Task<Guid> GetClientIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        return await GetCollection<Customer>().Find(filter).Project(x => x.ClientId).SingleOrDefaultAsync(cancellationToken);
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
            .Set(x => x.CountryId, customer.CountryId)
            .Set(x => x.Country, customer.Country)
            .Set(x => x.CurrencyId, customer.CurrencyId)
            .Set(x => x.CurrencyCode, customer.CurrencyCode);

        await UpdateAsync<Customer>(customer.Id, definition, cancellationToken);
    }
}
