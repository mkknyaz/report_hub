using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Identity;
using Exadel.ReportHub.Identity.Stores;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class IdentityServerRegistrations
{
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentityServer()
            .AddClientStore<IdentityClientStore>()
            .AddResourceStore<IdentityResourceStore>()
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            .AddProfileService<ProfileService>()
            .AddDeveloperSigningCredential(false);

        return services;
    }
}
