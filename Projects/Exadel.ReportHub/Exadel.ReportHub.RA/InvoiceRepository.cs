using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class InvoiceRepository : BaseRepository, IInvoiceRepository
{
    private static readonly FilterDefinitionBuilder<Invoice> _filterBuilder = Builders<Invoice>.Filter;

    public InvoiceRepository(MongoDbContext context)
        : base(context)
    {
    }

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
            .Set(x => x.Amount, invoice.Amount)
            .Set(x => x.IssueDate, invoice.IssueDate)
            .Set(x => x.DueDate, invoice.DueDate)
            .Set(x => x.BankAccountNumber, invoice.BankAccountNumber)
            .Set(x => x.PaymentStatus, invoice.PaymentStatus);
        return UpdateAsync(invoice.Id, definition, cancellationToken);
    }
}
