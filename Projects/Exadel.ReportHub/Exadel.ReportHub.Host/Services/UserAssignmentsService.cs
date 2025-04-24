using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.UserAssignment.Upsert;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.UserAssignment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/user-assignments")]
public class UserAssignmentsService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPost]
    [SwaggerOperation(Summary = "Upsert user assignment", Description = "Creates or updates the user assignment based on the provided data.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "User assignment was upserted successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid user assignment data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to perform this action")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> UpsertUserAssignment([FromBody] UpsertUserAssignmentDTO upsertUserAssignmentDto)
    {
        var result = await sender.Send(new UpsertUserAssignmentRequest(upsertUserAssignmentDto));
        return FromResult(result);
    }
}
