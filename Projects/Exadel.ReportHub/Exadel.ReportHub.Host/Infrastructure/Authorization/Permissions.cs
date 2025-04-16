using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.Host.Infrastructure.Authorization;

public static class Permissions
{
    private static Dictionary<string, Dictionary<Permission, List<UserRole>>> permissions = new()
    {
        {
            nameof(Client), new()
            {
                { Permission.Create, new() { UserRole.SuperAdmin } },
                { Permission.Read, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.SuperAdmin, UserRole.Owner } },
                { Permission.Delete, new() { UserRole.SuperAdmin } },
            }
        },
        {
            nameof(User), new()
            {
                { Permission.Create, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin } },
            }
        },
        {
            nameof(UserAssignment), new()
            {
                { Permission.Create, new() { UserRole.SuperAdmin } },
                { Permission.Read, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.SuperAdmin, UserRole.Owner } },
                { Permission.Delete, new() { UserRole.SuperAdmin } },
            }
        },
        {
            nameof(Item), new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner, UserRole.ClientAdmin } },
            }
        },
        {
            nameof(Invoice), new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner } },
            }
        },
        {
            nameof(Customer), new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner } },
            }
        },
        {
            nameof(Plan), new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner } },
            }
        },
        {
            nameof(Report), new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Update, new() { UserRole.Owner } },
                { Permission.Delete, new() { UserRole.Owner } },
            }
        },
    };

    public static IList<UserRole> GetAllowedRoles(string resource, Permission permission)
    {
        return permissions.GetValueOrDefault(resource)?.GetValueOrDefault(permission) ?? new List<UserRole>();
    }
}
