using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Plan.Create;
using Exadel.ReportHub.Handlers.Plan.Delete;
using Exadel.ReportHub.Handlers.Plan.GetByClientId;
using Exadel.ReportHub.Handlers.Plan.GetById;
using Exadel.ReportHub.Handlers.Plan.Update;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/plans")]
public class PlansService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    public async Task<ActionResult<PlanDTO>> AddPlan([FromBody] CreatePlanDTO createPlanDto)
    {
        var result = await sender.Send(new CreatePlanRequest(createPlanDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    public async Task<ActionResult<IList<PlanDTO>>> GetPlansByClientId([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetPlansByClientIdRequest(clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlanDTO>> GetPlanById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetPlanByIdRequest(id));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdatePlan([FromRoute] Guid id, [FromBody] UpdatePlanDTO updatePlanDateDto)
    {
        var result = await sender.Send(new UpdatePlanRequest(id, updatePlanDateDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeletePlan([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeletePlanRequest(id));
        return FromResult(result);
    }
}
