using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Exadel.ReportHub.Handlers.User.Create;
using Exadel.ReportHub.Host.Mediatr;
using FluentValidation;
using MediatR;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class MediatRRegistrations
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var assembly = typeof(CreateUserHandler).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidation(assembly);

        return services;
    }

    private static void AddValidation(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
    }
}
