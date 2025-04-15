using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IUserAssignmentRepository
{
    Task UpsertAsync(UserAssignment userAssignment, CancellationToken cancellationToken);

    Task<IEnumerable<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid userId, Guid clientId, CancellationToken cancellationToken);

    Task<bool> ExistAnyAsync(Guid userId, IEnumerable<Guid> clientIds, IEnumerable<UserRole> roles, CancellationToken cancellationToken);

    Task UpdateRoleAsync(Guid userId, Guid clientId, UserRole userRole, CancellationToken cancellationToken);

    Task<IEnumerable<Guid>> GetClientIdsAsync(Guid userId, CancellationToken cancellationToken);

    Task DeleteAsync(Guid userId, IEnumerable<Guid> clientIds, CancellationToken cancellationToken);
}
