﻿using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class UserAssignmentRepository(MongoDbContext context) : BaseRepository(context), IUserAssignmentRepository
{
    private static readonly FilterDefinitionBuilder<UserAssignment> _filterBuilder = Builders<UserAssignment>.Filter;

    public async Task UpsertAsync(UserAssignment userAssignment, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(_filterBuilder.Eq(x => x.UserId, userAssignment.UserId), _filterBuilder.Eq(x => x.ClientId, userAssignment.ClientId));
        var update = Builders<UserAssignment>.Update
            .Set(x => x.Role, userAssignment.Role)
            .SetOnInsert(x => x.Id, Guid.NewGuid());

        await GetCollection<UserAssignment>().UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
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

    public async Task<IList<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);
        var field = new ExpressionFieldDefinition<UserAssignment, UserRole>(x => x.Role);
        var userRoles = await GetCollection<UserAssignment>().DistinctAsync(field, filter, cancellationToken: cancellationToken);
        return await userRoles.ToListAsync(cancellationToken);
    }

    public Task<UserRole?> GetUserRoleByClientIdAsync(Guid userId, Guid clientId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId) &
                     _filterBuilder.Eq(x => x.ClientId, clientId);

        return GetCollection<UserAssignment>().Find(filter).Project(x => (UserRole?)x.Role).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateRoleAsync(Guid userId, Guid clientId, UserRole userRole, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(_filterBuilder.Eq(x => x.UserId, userId), _filterBuilder.Eq(x => x.ClientId, clientId));
        var update = Builders<UserAssignment>.Update.Set(x => x.Role, userRole);

        await GetCollection<UserAssignment>().UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async Task<IList<Guid>> GetClientIdsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);

        var clientIds = await GetCollection<UserAssignment>().Find(filter).Project(x => x.ClientId).ToListAsync(cancellationToken);
        return clientIds;
    }

    public Task DeleteAsync(Guid userId, IEnumerable<Guid> clientIds, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.And(_filterBuilder.Eq(x => x.UserId, userId), _filterBuilder.In(x => x.ClientId, clientIds));

        return GetCollection<UserAssignment>().DeleteManyAsync(filter, cancellationToken);
    }

    public Task<IList<UserAssignment>> GetUserAssignmentsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.UserId, userId);

        return GetAsync<UserAssignment>(filter, cancellationToken);
    }
}
