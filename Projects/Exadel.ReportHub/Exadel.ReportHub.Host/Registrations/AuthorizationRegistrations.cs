using Exadel.ReportHub.Host.Infrastructure.Authorization;
using Exadel.ReportHub.Host.PolicyHandlers;
using Microsoft.AspNetCore.Authorization;

namespace Exadel.ReportHub.Host.Registrations;

public static class AuthorizationRegistrations
{
    public static void AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationPolicy();
        services.AddAuthorizationHandlers();
    }

    private static void AddAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.Authorization.Policy.Create, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.Create)));
            options.AddPolicy(Constants.Authorization.Policy.Read, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.Read)));
            options.AddPolicy(Constants.Authorization.Policy.Update, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.Update)));
            options.AddPolicy(Constants.Authorization.Policy.Delete, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.Delete)));
        });
    }

    private static void AddAuthorizationHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
    }
}
