using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(MongoDbContext context)
        : base(context)
    {
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Email, email);
        return await GetCollection().Find(filter).SingleOrDefaultAsync(cancellationToken);
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
        var isActive = await GetCollection().Find(filter).Project(x => x.IsActive).SingleOrDefaultAsync(cancellationToken);
        return isActive;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        var count = await GetCollection().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }
}
