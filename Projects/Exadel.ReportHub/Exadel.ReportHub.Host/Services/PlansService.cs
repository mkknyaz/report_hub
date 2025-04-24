using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Plan.Create;
using Exadel.ReportHub.Handlers.Plan.Delete;
using Exadel.ReportHub.Handlers.Plan.GetByClientId;
using Exadel.ReportHub.Handlers.Plan.GetById;
using Exadel.ReportHub.Handlers.Plan.Update;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/plans")]
public class PlansService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new plan", Description = "Creates a new plan and returns its details.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Plan created successfully", typeof(ActionResult<PlanDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid plan data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to create the plan")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<PlanDTO>> AddPlan([FromBody] CreatePlanDTO createPlanDto)
    {
        var result = await sender.Send(new CreatePlanRequest(createPlanDto));
        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get plans by client id", Description = "Returns a list of plans for the specified client.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plans retrieved successfully", typeof(ActionResult<IList<PlanDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to view plans")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<IList<PlanDTO>>> GetPlansByClientId([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetPlansByClientIdRequest(clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get plan by id", Description = "Returns the details of the specified plan for the given client.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plan details retrieved successfully", typeof(ActionResult<PlanDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to view the plan")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plan was not found for the given id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<PlanDTO>> GetPlanById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetPlanByIdRequest(id));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update plan", Description = "Updates the plan with the specified id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Plan updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid plan data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to update the plan")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plan was not found for the specified id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> UpdatePlan([FromRoute] Guid id, [FromBody] UpdatePlanDTO updatePlanDateDto)
    {
        var result = await sender.Send(new UpdatePlanRequest(id, updatePlanDateDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete plan", Description = "Deletes the plan with the specified id and client.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Plan was deleted successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid plan data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to delete the plan")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plan was not found for the specified id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> DeletePlan([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeletePlanRequest(id));
        return FromResult(result);
    }
}
