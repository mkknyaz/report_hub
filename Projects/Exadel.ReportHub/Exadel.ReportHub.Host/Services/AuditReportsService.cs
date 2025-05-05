using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Audit.GetById;
using Exadel.ReportHub.Handlers.Audit.GetByUserId;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.AuditReport;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.Pagination;
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
    [SwaggerOperation(Summary = "Get paginated audit reports by user ID",
        Description = "Retrieves a specific audit report using their user's identifier. Supports pagination using Top and Skip")]
    [SwaggerResponse(StatusCodes.Status200OK, "Audit reports were retrieved successfully", typeof(ActionResult<PageResultDTO<AuditReportDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User doesnt exist", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<PageResultDTO<AuditReportDTO>>> GetAuditReportsByUserId(
        [FromQuery][Required] Guid userId,
        [FromQuery][Required] Guid clientId,
        [FromQuery][Required] PageRequestDTO pageRequestDto)
    {
            var result = await sender.Send(new GetAuditReportsByUserIdRequest(userId, pageRequestDto));
            return FromResult(result);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get audit report by id", Description = "Retrieves a specific audit report using their unique identifier")]
    [SwaggerResponse(StatusCodes.Status200OK, "Audit report was retrieved successfully", typeof(ActionResult<AuditReportDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Audit report was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<AuditReportDTO>> GetAuditReportById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetAuditReportByIdRequest(id));
        return FromResult(result);
    }
}
