using Exadel.ReportHub.Data.Enums;

namespace Exadel.ReportHub.Host.Infrastructure.Authorization;

public static class Permissions
{
    private static Dictionary<string, Dictionary<Permission, List<UserRole>>> permissions = new()
    {
        {
            Constants.Authorization.Resource.Clients, new()
            {
                { Permission.Create, new() { UserRole.SuperAdmin } },
                { Permission.Read, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.SuperAdmin, UserRole.Owner } },
                { Permission.Delete, new() { UserRole.SuperAdmin } },
            }
        },
        {
            Constants.Authorization.Resource.Users, new()
            {
                { Permission.Create, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin } },
            }
        },
        {
            Constants.Authorization.Resource.UserAssignments, new()
            {
                { Permission.Create, new() { UserRole.SuperAdmin } },
                { Permission.Read, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.SuperAdmin, UserRole.Owner } },
                { Permission.Delete, new() { UserRole.SuperAdmin } },
            }
        },
        {
            Constants.Authorization.Resource.Items, new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner, UserRole.ClientAdmin } },
            }
        },
        {
            Constants.Authorization.Resource.Invoices, new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner } },
                { Permission.Export, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } }
            }
        },
        {
            Constants.Authorization.Resource.Customers, new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner } },
            }
        },
        {
            Constants.Authorization.Resource.Plans, new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Update, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Delete, new() { UserRole.Owner } },
            }
        },
        {
            Constants.Authorization.Resource.Reports, new()
            {
                { Permission.Create, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Read, new() { UserRole.Owner, UserRole.ClientAdmin } },
                { Permission.Update, new() { UserRole.Owner } },
                { Permission.Delete, new() { UserRole.Owner } },
                { Permission.Export, new() { UserRole.Owner, UserRole.ClientAdmin } }
            }
        },
        {
            Constants.Authorization.Resource.AuditReports, new()
            {
                { Permission.Read, new() { UserRole.SuperAdmin, UserRole.Owner, UserRole.ClientAdmin, UserRole.Operator } },
            }
        }
    };

    public static IList<UserRole> GetAllowedRoles(string resource, Permission permission)
    {
        return permissions.GetValueOrDefault(resource)?.GetValueOrDefault(permission) ?? new();
    }
}
