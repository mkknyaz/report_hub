using Exadel.ReportHub.Identity;
using Exadel.ReportHub.Identity.Stores;

namespace Exadel.ReportHub.Host.Registrations;

public static class IdentityServerRegistrations
{
    public static void AddIdentity(this IServiceCollection services)
    {
        services.AddIdentityServer()
            .AddClientStore<IdentityClientStore>()
            .AddResourceStore<IdentityResourceStore>()
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            .AddDeveloperSigningCredential(false);
    }
}
