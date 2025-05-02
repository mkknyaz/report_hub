using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class InvoiceRepository(MongoDbContext context) : BaseRepository(context), IInvoiceRepository
{
    private static readonly FilterDefinitionBuilder<Invoice> _filterBuilder = Builders<Invoice>.Filter;

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        return base.AddAsync(invoice, cancellationToken);
    }

    public Task AddManyAsync(IEnumerable<Invoice> invoices, CancellationToken cancellationToken)
    {
        return base.AddManyAsync(invoices, cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExistsAsync<Invoice>(id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string invoiceNumber, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.InvoiceNumber, invoiceNumber);
        var count = await GetCollection<Invoice>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }

    public Task<IList<Invoice>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Eq(x => x.IsDeleted, false));
        return GetAsync(filter, cancellationToken);
    }

    public Task<Invoice> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync<Invoice>(id, cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return SoftDeleteAsync<Invoice>(id, cancellationToken);
    }

    public Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        var definition = Builders<Invoice>.Update
            .Set(x => x.IssueDate, invoice.IssueDate)
            .Set(x => x.DueDate, invoice.DueDate);
        return UpdateAsync(invoice.Id, definition, cancellationToken);
    }

    public async Task<(string CurrencyCode, decimal Total)> SumOfClientAmountAsync(Guid clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Gte(x => x.IssueDate, startDate),
            _filterBuilder.Lte(x => x.IssueDate, endDate),
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Eq(x => x.IsDeleted, false));

        var result = await GetCollection<Invoice>()
            .Aggregate()
            .Match(filter)
            .Group(x => x.ClientCurrencyCode, g => new
            {
                Currency = g.Key,
                Total = g.Sum(x => x.ClientCurrencyAmount)
            })
            .SingleOrDefaultAsync(cancellationToken);

        return (result.Currency, result.Total);
    }
}
