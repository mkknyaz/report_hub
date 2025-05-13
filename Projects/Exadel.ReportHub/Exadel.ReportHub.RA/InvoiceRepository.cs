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

    public async Task<Invoice> GetByIdAsync(Guid id, Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.Id, id),
            _filterBuilder.Eq(x => x.ClientId, clientId),
            _filterBuilder.Eq(x => x.IsDeleted, false));

        return await GetCollection<Invoice>().Find(filter).SingleOrDefaultAsync(cancellationToken);
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

    public async Task<Dictionary<Guid, int>> GetClientItemsCountAsync(Guid clientId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.ClientId, clientId);
        if (startDate.HasValue)
        {
            filter &= _filterBuilder.Gte(x => x.IssueDate, startDate.Value);
        }

        if (endDate.HasValue)
        {
            filter &= _filterBuilder.Lte(x => x.IssueDate, endDate.Value);
        }

        var grouping = await GetCollection<Invoice>()
            .Aggregate().
            Match(filter)
            .Unwind<Invoice, UnwoundInvoice>(x => x.ItemIds)
            .Group(x => x.ItemIds, g => new { ItemId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        return grouping.ToDictionary(x => x.ItemId, x => x.Count);
    }

    public async Task<Dictionary<Guid, int>> GetPlansActualCountAsync(IEnumerable<Plan> plans, CancellationToken cancellationToken)
    {
        var planList = plans.ToList();
        var facets = planList
            .Select(plan =>
                AggregateFacet.Create(
                    plan.Id.ToString(),
                    PipelineDefinition<Invoice, AggregateCountResult>.Create([
                        PipelineStageDefinitionBuilder.Match(
                            _filterBuilder.AnyEq(x => x.ItemIds, plan.ItemId) &
                            _filterBuilder.Gte(x => x.IssueDate, plan.StartDate) &
                            _filterBuilder.Lte(x => x.IssueDate, plan.EndDate)),
                        PipelineStageDefinitionBuilder.Count<Invoice>()
                    ])))
            .ToList();

        var facetResults = await GetCollection<Invoice>()
            .Aggregate()
            .Facet(facets)
            .SingleAsync(cancellationToken);

        return facetResults.Facets.ToDictionary(
            x => Guid.Parse(x.Name),
            x => x.Output<AggregateCountResult>().Any() ?
                (int)x.Output<AggregateCountResult>()[0].Count : 0);
    }

    public async Task<InvoicesReport> GetReportAsync(Guid clientId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.ClientId, clientId);
        if (startDate.HasValue)
        {
            filter &= _filterBuilder.Gte(x => x.IssueDate, startDate.Value);
        }

        if (endDate.HasValue)
        {
            filter &= _filterBuilder.Lte(x => x.IssueDate, endDate.Value);
        }

        var facetMainStatistics = AggregateFacet.Create(
            "MainStatistics",
            PipelineDefinition<Invoice, ReportMainStatistics>.Create([
                PipelineStageDefinitionBuilder.Match(filter),
                PipelineStageDefinitionBuilder.Group<Invoice, bool, ReportMainStatistics>(
                    _ => true,
                    g => new ReportMainStatistics(
                        g.Count(),
                        g.Sum(x => x.ClientCurrencyAmount),
                        g.Average(x => x.ClientCurrencyAmount)))
            ]));

        var facetMonthCount = AggregateFacet.Create(
            "MonthCount",
            PipelineDefinition<Invoice, MonthCount>.Create([
                PipelineStageDefinitionBuilder.Match(filter),
                PipelineStageDefinitionBuilder.Group<Invoice, YearMonth, MonthCount>(
                    x => new YearMonth(x.IssueDate.Year, x.IssueDate.Month),
                    g => new MonthCount(
                        g.Key,
                        g.Count()))
            ]));

        var facetStatusCount = AggregateFacet.Create(
            "StatusCount",
            PipelineDefinition<Invoice, StatusCount>.Create([
                PipelineStageDefinitionBuilder.Match(filter),
                PipelineStageDefinitionBuilder.Group<Invoice, PaymentStatus, StatusCount>(
                    x => x.PaymentStatus,
                    g => new StatusCount(
                        g.Key,
                        g.Count()))
                ]));

        var facetResults = await GetCollection<Invoice>()
            .Aggregate()
            .Facet(facetMainStatistics, facetMonthCount, facetStatusCount)
            .SingleAsync(cancellationToken);

        var mainStatistics = facetResults.Facets[0]
            .Output<ReportMainStatistics>()
            .SingleOrDefault();

        if (mainStatistics == null)
        {
            return null;
        }

        var monthCounts = facetResults.Facets[1]
            .Output<MonthCount>();

        var statusCounts = facetResults.Facets[2]
            .Output<StatusCount>()
            .ToDictionary(x => x.Status, x => x.Count);

        statusCounts.TryGetValue(PaymentStatus.Unpaid, out var unpaidCount);
        statusCounts.TryGetValue(PaymentStatus.Overdue, out var overdueCount);
        statusCounts.TryGetValue(PaymentStatus.PaidOnTime, out var paidOnTimeCount);
        statusCounts.TryGetValue(PaymentStatus.PaidLate, out var paidLateCount);

        return new InvoicesReport
        {
            TotalCount = mainStatistics.TotalCount,
            AverageMonthCount = monthCounts.Any() ?
                (int)Math.Round(monthCounts.Average(x => x.Count)) : 0,
            TotalAmount = mainStatistics.TotalAmount,
            AverageAmount = mainStatistics.AverageAmount,
            UnpaidCount = unpaidCount,
            OverdueCount = overdueCount,
            PaidOnTimeCount = paidOnTimeCount,
            PaidLateCount = paidLateCount
        };
    }

    private sealed record UnwoundInvoice(Guid ItemIds, DateTime IssueDate);

    private sealed record ReportMainStatistics(int TotalCount, decimal TotalAmount, decimal AverageAmount);

    private sealed record YearMonth(int Year, int Month);

    private sealed record MonthCount(YearMonth Month, int Count);

    private sealed record StatusCount(PaymentStatus Status, int Count);
}
