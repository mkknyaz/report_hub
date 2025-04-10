using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class UserRepository : BaseRepository, IUserRepository
{
    private static readonly FilterDefinitionBuilder<User> _filterBuilder = Builders<User>.Filter;

    public UserRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Email, email);
        var count = await GetCollection<User>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }

    public async Task<IEnumerable<User>> GetAllActiveAsync(CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.IsActive, true);
        return await GetAsync(filter, cancellationToken);
    }

    public async Task UpdateActivityAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update.Set(x => x.IsActive, isActive);
        await UpdateAsync(id, update, cancellationToken);
    }

    public async Task<bool> IsActiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        var isActive = await GetCollection<User>().Find(filter).Project(x => x.IsActive).SingleOrDefaultAsync(cancellationToken);
        return isActive;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await ExistsAsync<User>(id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await base.AddAsync(user, cancellationToken);
    }

    public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await GetByIdAsync<User>(id, cancellationToken);
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Email, email);
        return await GetCollection<User>().Find(filter).SingleOrDefaultAsync();
    }

    public async Task UpdateRoleAsync(Guid id, UserRole userRole, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update.Set(x => x.Role, userRole);
        await UpdateAsync(id, update, cancellationToken);
    }

    public async Task UpdatePasswordAsync(Guid id, string passwordHash, string passwordSalt, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update
            .Set(x => x.PasswordHash, passwordHash)
            .Set(x => x.PasswordSalt, passwordSalt);
        await UpdateAsync(id, update, cancellationToken);
    }
}
