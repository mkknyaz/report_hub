using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class PlanRepository(MongoDbContext context) : BaseRepository(context), IPlanRepository
{
    private static readonly FilterDefinitionBuilder<Plan> _filterBuilder = Builders<Plan>.Filter;

    public Task AddAsync(Plan plan, CancellationToken cancellationToken)
    {
        return AddAsync<Plan>(plan, cancellationToken);
    }

    public Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return SoftDeleteAsync<Plan>(id, cancellationToken);
    }

    public Task<IList<Plan>> GetByClientIdAsync(Guid clientId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.IsDeleted, false),
            _filterBuilder.Eq(x => x.ClientId, clientId));
        if (startDate.HasValue)
        {
            filter &= _filterBuilder.Gte(x => x.StartDate, startDate.Value);
        }

        if (endDate.HasValue)
        {
            filter &= _filterBuilder.Lte(x => x.EndDate, endDate.Value);
        }

        return GetAsync(filter, cancellationToken);
    }

    public Task<Plan> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync<Plan>(id, cancellationToken);
    }

    public Task UpdateDateAsync(Guid id, Plan plan, CancellationToken cancellationToken)
    {
        var update = Builders<Plan>.Update
            .Set(x => x.StartDate, plan.StartDate)
            .Set(x => x.EndDate, plan.EndDate)
            .Set(x => x.Count, plan.Count);
        return UpdateAsync(id, update, cancellationToken);
    }

    public async Task<bool> ExistsForItemByPeriodAsync(Guid itemId, Guid clientId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var filter =
            _filterBuilder.Eq(x => x.ClientId, clientId) &
            _filterBuilder.Eq(x => x.ItemId, itemId) &
            (_filterBuilder.Gte(x => x.EndDate, startDate) |
            _filterBuilder.Lte(x => x.StartDate, endDate)) &
            _filterBuilder.Eq(x => x.IsDeleted, false);
        var count = await GetCollection<Plan>().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExistsAsync<Plan>(id, cancellationToken);
    }
}
