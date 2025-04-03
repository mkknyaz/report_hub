using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);

    Task<IEnumerable<User>> GetAllActiveAsync(CancellationToken cancellationToken);

    Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    Task UpdateActivityAsync(Guid id, bool isActive, CancellationToken cancellationToken);

    Task<bool> IsActiveAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateRoleAsync(Guid id, UserRole userRole, CancellationToken cancellationToken);
}
