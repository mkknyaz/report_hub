using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Bson;
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

    public async Task<bool> ExistsAsync(Guid id, Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.Id, id),
            _filterBuilder.Eq(x => x.ClientId, clientId));
        var count = await GetCollection<Invoice>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
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

    public async Task UpdatePaidStatusAsync(Guid id, Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.Id, id),
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.In(x => x.PaymentStatus, new[] { PaymentStatus.Unpaid, PaymentStatus.Overdue }));

        PipelineDefinition<Invoice, Invoice> pipeline = new[]
        {
            new BsonDocument("$set", new BsonDocument(nameof(PaymentStatus),
            new BsonDocument("$cond", new BsonArray
            {
                new BsonDocument("$eq", new BsonArray { $"${nameof(PaymentStatus)}", PaymentStatus.Unpaid.ToString() }),
                PaymentStatus.PaidOnTime.ToString(),
                PaymentStatus.PaidLate.ToString()
            })))
        };

        await GetCollection<Invoice>().UpdateOneAsync(filter, pipeline, cancellationToken: cancellationToken);
    }

    public async Task<long> UpdateOverdueStatusAsync(DateTime date, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.PaymentStatus, PaymentStatus.Unpaid),
            _filterBuilder.Lt(x => x.DueDate, date),
            _filterBuilder.Eq(x => x.IsDeleted, false));
        var updateDefinition = Builders<Invoice>.Update.Set(x => x.PaymentStatus, PaymentStatus.Overdue);

        var result = await GetCollection<Invoice>().UpdateManyAsync(filter, updateDefinition, cancellationToken: cancellationToken);
        return result.ModifiedCount;
    }

    public async Task<TotalRevenue> GetTotalAmountByDateRangeAsync(Guid clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Gte(x => x.IssueDate, startDate),
            _filterBuilder.Lte(x => x.IssueDate, endDate),
            _filterBuilder.Eq(x => x.IsDeleted, false));

        var result = await GetCollection<Invoice>()
            .Aggregate()
            .Match(filter)
            .Group(x => x.ClientCurrencyCode, g => new TotalRevenue
            {
                TotalAmount = g.Sum(x => x.ClientCurrencyAmount),
                CurrencyCode = g.Key,
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return new TotalRevenue();
        }

        return result;
    }

    public async Task<Dictionary<Guid, int>> GetCountByDateRangeAsync(DateTime startDate, DateTime endDate, Guid clientId, Guid? customerId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Gte(x => x.IssueDate, startDate),
            _filterBuilder.Lte(x => x.IssueDate, endDate),
            _filterBuilder.Eq(x => x.IsDeleted, false));

        if (customerId.HasValue)
        {
            filter &= _filterBuilder.Eq(x => x.CustomerId, customerId);
        }

        var grouping = await GetCollection<Invoice>().Aggregate().Match(filter).Group(x => x.CustomerId, g => new { CustomerId = g.Key, Count = g.Count() }).ToListAsync(cancellationToken);
        return grouping.ToDictionary(x => x.CustomerId, x => x.Count);
    }

    public async Task<OverdueCount> GetOverdueAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Eq(x => x.PaymentStatus, PaymentStatus.Overdue),
            _filterBuilder.Eq(x => x.IsDeleted, false));

        var result = await GetCollection<Invoice>()
            .Aggregate()
            .Match(filter)
            .Group(x => x.ClientCurrencyCode, g => new OverdueCount
            {
                Count = g.Count(),
                TotalAmount = g.Sum(x => x.ClientCurrencyAmount),
                ClientCurrencyCode = g.Key,
            })
            .SingleOrDefaultAsync(cancellationToken);
        return result;
    }
}
