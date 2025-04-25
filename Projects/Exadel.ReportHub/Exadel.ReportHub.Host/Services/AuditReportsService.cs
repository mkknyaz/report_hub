using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Audit.GetById;
using Exadel.ReportHub.Handlers.Audit.GetByUserId;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.AuditReport;
using Exadel.ReportHub.SDK.DTOs.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/audit-reports")]
[Authorize(Policy = Constants.Authorization.Policy.Read)]
public class AuditReportsService(ISender sender) : BaseService
{
    [HttpGet]
    [SwaggerOperation(Summary = "Get audit reports by user id", Description = "Retrieves a specific audit report using their user's identifier")]
    [SwaggerResponse(StatusCodes.Status200OK, "Audit reports were retrieved successfully", typeof(ActionResult<ClientDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User doesnt exist", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<IList<AuditReportDTO>>> GetAuditReportsByUserId([FromQuery][Required] Guid userId, [FromQuery][Required] Guid clientId)
    {
            var result = await sender.Send(new GetAuditReportsByUserIdRequest(userId));
            return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get audit report by id", Description = "Retrieves a specific audit report using their unique identifier")]
    [SwaggerResponse(StatusCodes.Status200OK, "Audit report was retrieved successfully", typeof(ActionResult<ClientDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Audit report was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<AuditReportDTO>> GetAuditReportById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetAuditReportByIdRequest(id));
        return FromResult(result);
    }
}
