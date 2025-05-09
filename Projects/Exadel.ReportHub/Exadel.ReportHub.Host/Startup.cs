using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using AutoMapper;
using Exadel.ReportHub.Host.Infrastructure.Filters;
using Exadel.ReportHub.Host.Registrations;
using Exadel.ReportHub.SDK.Abstract;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;

namespace Exadel.ReportHub.Host;

[ExcludeFromCodeCoverage]
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
                            { Constants.Authorization.ScopeName, Constants.Authorization.ScopeDescription }
                        }
                    },
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = tokenUrl,
                        Scopes = new Dictionary<string, string>
                        {
                            { Constants.Authorization.ScopeName, Constants.Authorization.ScopeDescription }
                        }
                    }
                }
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    new[] { Constants.Authorization.ScopeName }
                }
            });
            c.EnableAnnotations();
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = configuration["Authority"];
                options.Audience = Constants.Authorization.ResourceName;
            });

        AuthorizationRegistrations.AddAuthorization(services);

        services
            .AddIdentity()
            .AddMongo()
            .AddRepositories()
            .AddMediatR()
            .AddAutoMapper(typeof(Startup))
            .AddHttpContextAccessor()
            .AddCsv()
            .AddPdf()
            .AddEmailSender(configuration)
            .AddExchangeRate(configuration)
            .AddPing(configuration)
            .AddScheduler()
            .AddJobs()
            .AddHangfire()
            .AddManagers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
    {
        mapper.ConfigurationProvider.AssertConfigurationIsValid();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Report Hub API"));
        app.UseRouting();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedProto
        });

        app.UseIdentityServer();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseHangfireDashboard();
        app.ApplicationServices.GetRequiredService<ISchedulerService>().StartJobs();
    }
}
