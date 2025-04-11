using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

public class UserAssignmentRepository : BaseRepository, IUserAssignmentRepository
{
    private static readonly FilterDefinitionBuilder<UserAssignment> _filterBuilder = Builders<UserAssignment>.Filter;

    public UserAssignmentRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task AddAsync(UserAssignment userAssignment, CancellationToken cancellationToken)
    {
        await base.AddAsync(userAssignment, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(_filterBuilder.Eq(x => x.UserId, userId), _filterBuilder.Eq(x => x.ClientId, clientId));
        var count = await GetCollection<UserAssignment>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }

    public async Task<bool> ExistAnyAsync(Guid userId, IEnumerable<Guid> clientIds, IEnumerable<UserRole> roles, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(
            _filterBuilder.Eq(x => x.UserId, userId),
            _filterBuilder.In(x => x.ClientId, clientIds),
            _filterBuilder.In(x => x.Role, roles));
        var count = await GetCollection<UserAssignment>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);
        var field = new ExpressionFieldDefinition<UserAssignment, UserRole>(x => x.Role);
        var userRoles = await GetCollection<UserAssignment>().DistinctAsync(field, filter);
        return await userRoles.ToListAsync();
    }

    public async Task UpdateRoleAsync(Guid userId, Guid clientId, UserRole userRole, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(_filterBuilder.Eq(x => x.UserId, userId), _filterBuilder.Eq(x => x.ClientId, clientId));
        var update = Builders<UserAssignment>.Update.Set(x => x.Role, userRole);
        await GetCollection<UserAssignment>().UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }
}
