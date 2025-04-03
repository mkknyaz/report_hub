using System.Text.Json.Serialization;
using AutoMapper;
using Exadel.ReportHub.Host.Infrastructure.Filters;
using Exadel.ReportHub.Host.Registrations;
using Microsoft.OpenApi.Models;

namespace Exadel.ReportHub.Host;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddSwaggerGen(c =>
        {
            const string apiVersion = "v1";
            const string scopeName = "report_hub_api";
            const string scopeDescription = "Full access to Report Hub API";

            var tokenUrl = new Uri($"{configuration["Authority"]}/connect/token");

            c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = "ReportHubAPI", Version = apiVersion });
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        TokenUrl = tokenUrl,
                        Scopes = new Dictionary<string, string>
                        {
                            { scopeName, scopeDescription }
                        }
                    },
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = tokenUrl,
                        Scopes = new Dictionary<string, string>
                        {
                            { scopeName, scopeDescription }
                        }
                    }
                }
            });
        });

        services.AddAuthorization();

        services.AddIdentity();
        services.AddMongo();
        services.AddMediatR();
        services.AddAutoMapper(typeof(Startup));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Report Hub API"));
        app.UseRouting();

        app.UseIdentityServer();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
