using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using AutoMapper;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Host.Infrastructure.Filters;
using Exadel.ReportHub.Host.PolicyHandlers;
using Exadel.ReportHub.Host.Registrations;
using Exadel.ReportHub.RA;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.Authorization.Policy.AllUsers, policy =>
                policy.Requirements.Add(new ClientAssignmentRequirement(UserRole.Regular)));
            options.AddPolicy(Constants.Authorization.Policy.ClientAdmin, policy =>
                policy.Requirements.Add(new ClientAssignmentRequirement(UserRole.ClientAdmin)));
            options.AddPolicy(Constants.Authorization.Policy.SuperAdmin, policy =>
                policy.Requirements.Add(new ClientAssignmentRequirement()));
        });

        services.AddIdentity();
        services.AddMongo();
        services.AddMediatR();
        services.AddAutoMapper(typeof(Startup));
        services.AddHttpContextAccessor();
        services.AddScoped<IUserProvider, UserProvider>();
        services.AddSingleton<IAuthorizationHandler, ClientAssignmentHandler>();
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
    }
}
