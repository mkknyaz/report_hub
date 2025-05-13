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
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.UserAssignment.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpsertUserAssignment([FromBody] UpsertUserAssignmentDTO upsertUserAssignmentDto)
    {
        var result = await sender.Send(new UpsertUserAssignmentRequest(upsertUserAssignmentDto));
        return FromResult(result);
    }
}
