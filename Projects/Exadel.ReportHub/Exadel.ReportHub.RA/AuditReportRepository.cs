using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

public class AuditReportRepository : BaseRepository, IAuditReportRepository
{
    private static readonly FilterDefinitionBuilder<AuditReport> _filterBuilder = Builders<AuditReport>.Filter;

    public AuditReportRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task<AuditReport> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetByIdAsync<AuditReport>(id, cancellationToken);
    }

    public async Task<IList<AuditReport>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);
        var desc = await GetCollection<AuditReport>()
            .Find(filter)
            .SortByDescending(x => x.TimeStamp)
            .ToListAsync(cancellationToken);
        return desc;
    }

    public async Task AddAsync(AuditReport auditReport, CancellationToken cancellationToken)
    {
        await base.AddAsync(auditReport, cancellationToken);
    }
}
