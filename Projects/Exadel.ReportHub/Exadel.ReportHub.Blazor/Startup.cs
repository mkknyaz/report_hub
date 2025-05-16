using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Blazor.Components;

namespace Exadel.ReportHub.Blazor;

[ExcludeFromCodeCoverage]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        });
    }
}
