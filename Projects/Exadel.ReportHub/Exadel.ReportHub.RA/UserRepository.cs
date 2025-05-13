using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.RA.Extensions;
using MongoDB.Driver;

namespace Exadel.ReportHub.RA;

[ExcludeFromCodeCoverage]
public class UserRepository(MongoDbContext context) : BaseRepository(context), IUserRepository
{
    private static readonly FilterDefinitionBuilder<User> _filterBuilder = Builders<User>.Filter;

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Email, email);
        var count = await GetCollection<User>().Find(filter).CountDocumentsAsync(cancellationToken);
        return count > 0;
    }

    public Task<IList<User>> GetAsync(bool? isActive, CancellationToken cancellationToken)
    {
        var filter = isActive.HasValue
            ? _filterBuilder.Eq(x => x.IsActive, isActive.Value)
            : _filterBuilder.Empty;

        return GetAsync(filter, cancellationToken);
    }

    public Task UpdateActivityAsync(Guid id, bool isActive, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update.Set(x => x.IsActive, isActive);
        return UpdateAsync(id, update, cancellationToken);
    }

    public async Task<bool> IsActiveAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Id, id);
        var isActive = await GetCollection<User>().Find(filter).Project(x => x.IsActive).SingleOrDefaultAsync(cancellationToken);
        return isActive;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return ExistsAsync<User>(id, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        return base.AddAsync(user, cancellationToken);
    }

    public Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return GetByIdAsync<User>(id, cancellationToken);
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var filter = _filterBuilder.Eq(x => x.Email, email).Active();
        return await GetCollection<User>().Find(filter).SingleOrDefaultAsync(cancellationToken);
    }

    public Task UpdatePasswordAsync(Guid id, string passwordHash, string passwordSalt, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update
            .Set(x => x.PasswordHash, passwordHash)
            .Set(x => x.PasswordSalt, passwordSalt);
        return UpdateAsync(id, update, cancellationToken);
    }

    public Task UpdateNameAsync(Guid id, string fullName, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update.Set(x => x.FullName, fullName);
        return UpdateAsync(id, update, cancellationToken);
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return DeleteAsync<User>(id, cancellationToken);
    }

    public async Task<IList<User>> GetUsersByNotificationSettingsAsync(int dayOfMonth, DayOfWeek dayOfWeek, int hour, CancellationToken cancellationToken)
    {
        var baseFilter = _filterBuilder.Eq(u => u.NotificationSettings.Hour, hour).Active();

        var dailyFilter = _filterBuilder.Eq(u => u.NotificationSettings.Frequency, NotificationFrequency.Daily);

        var weeklyFilter = _filterBuilder.And(
            _filterBuilder.Eq(u => u.NotificationSettings.Frequency, NotificationFrequency.Weekly),
            _filterBuilder.Eq(u => u.NotificationSettings.DayOfWeek, dayOfWeek));

        var monthlyFilter = _filterBuilder.And(
            _filterBuilder.Eq(u => u.NotificationSettings.Frequency, NotificationFrequency.Monthly),
            _filterBuilder.Eq(u => u.NotificationSettings.DayOfMonth, dayOfMonth));

        var finalFilter = baseFilter & (dailyFilter | weeklyFilter | monthlyFilter);
        var result = await GetCollection<User>().Find(finalFilter).ToListAsync(cancellationToken);

        return result;
    }

    public Task UpdateNotificationSettingsAsync(Guid id, NotificationSettings notificationSettings, CancellationToken cancellationToken)
    {
        var update = Builders<User>.Update
            .Set(x => x.NotificationSettings, notificationSettings);
        return UpdateAsync(id, update, cancellationToken);
    }
}
