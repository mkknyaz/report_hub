using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.UserAssignment.Upsert;
using Exadel.ReportHub.SDK.DTOs.UserAssignment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
public class UserAssignmentService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> UpsertUserAssignment([FromBody] UpsertUserAssignmentDTO upsertUserAssignmentDto)
    {
        var result = await sender.Send(new UpsertUserAssignmentRequest(upsertUserAssignmentDto));
        return FromResult(result);
    }
}
