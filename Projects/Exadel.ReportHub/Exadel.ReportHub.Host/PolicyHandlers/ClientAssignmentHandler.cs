using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityServer.Extensions;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Host.Services;
using Exadel.ReportHub.RA.Abstract;
using Microsoft.AspNetCore.Authorization;

namespace Exadel.ReportHub.Host.PolicyHandlers;

public class ClientAssignmentRequirement : IAuthorizationRequirement
{
    public IList<UserRole> Roles { get; }

    public ClientAssignmentRequirement(params UserRole[] roles)
    {
        Roles = roles.ToList();
        if (!Roles.Contains(UserRole.SuperAdmin))
        {
            Roles.Add(UserRole.SuperAdmin);
        }
    }
}

public class ClientAssignmentHandler(
        IHttpContextAccessor httpContextAccessor,
        IUserAssignmentRepository userAssignmentRepository,
        ILogger<ClientAssignmentHandler> logger) : AuthorizationHandler<ClientAssignmentRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ClientAssignmentRequirement requirement)
    {
        Claim userIdClaim;
        if (!context.User.IsAuthenticated() ||
            (userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)) == null)
        {
            return;
        }

        var matchingRoles = requirement.Roles.Where(r => context.User.IsInRole(r.ToString())).ToList();
        if (matchingRoles.Count == 0)
        {
            return;
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var clientIds = new List<Guid> { Constants.Client.GlobalId };
        var requestClientId = await GetClientIdFromRequestAsync(httpContextAccessor.HttpContext.Request, logger);

        if (requestClientId.HasValue)
        {
            clientIds.Add(requestClientId.Value);
        }

        if (await userAssignmentRepository.ExistAnyAsync(userId, clientIds, matchingRoles, CancellationToken.None))
        {
            context.Succeed(requirement);
        }
    }

    private async Task<Guid?> GetClientIdFromRequestAsync(HttpRequest request, ILogger logger)
    {
        if (request.RouteValues.TryGetValue("controller", out var serviceName) &&
            serviceName.ToString().Equals(typeof(ClientService).Name, StringComparison.Ordinal) &&
            request.RouteValues.TryGetValue("id", out var routeClientIdObj) &&
            Guid.TryParse(routeClientIdObj.ToString(), out var routeClientId))
        {
            return routeClientId;
        }

        if (request.Query.TryGetValue("clientId", out var queryClientIdObj) &&
            Guid.TryParse(queryClientIdObj.ToString(), out var queryClientId))
        {
            return queryClientId;
        }

        if (request.ContentType?.Contains("application/json", StringComparison.Ordinal) == true)
        {
            request.EnableBuffering();
            try
            {
                var container = await JsonSerializer.DeserializeAsync<ClientIdContainer>(
                    request.BodyReader.AsStream(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    request.HttpContext.RequestAborted);

                return container?.ClientId;
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "No json-object found in body.");
            }
            finally
            {
                request.Body.Position = 0;
            }
        }

        return null;
    }

    private sealed record ClientIdContainer(Guid? ClientId);
}
