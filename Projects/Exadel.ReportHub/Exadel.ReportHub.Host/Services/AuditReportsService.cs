using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Audit.GetById;
using Exadel.ReportHub.Handlers.Audit.GetByUserId;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.AuditReport;
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
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.AuditReport.Status200RetrieveDescription, typeof(ActionResult<PageResultDTO<AuditReportDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.User.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
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
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.AuditReport.Status200RetrieveDescription, typeof(ActionResult<AuditReportDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.AuditReport.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<AuditReportDTO>> GetAuditReportById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetAuditReportByIdRequest(id));
        return FromResult(result);
    }
}
