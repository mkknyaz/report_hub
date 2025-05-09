using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Report.Invoices;
using Exadel.ReportHub.Handlers.Report.Items;
using Exadel.ReportHub.Handlers.Report.Plans;
using Exadel.ReportHub.Handlers.Report.Send;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.Abstract;
using Exadel.ReportHub.SDK.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/reports")]
[ApiController]
public class ReportsService(ISender sender) : BaseService, IReportService
{
    [Authorize(Policy = Constants.Authorization.Policy.Export)]
    [HttpGet("invoices")]
    [SwaggerOperation(Summary = "Export invoices report", Description = "Generates and exports a report of client invoices in required format using the provided client id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Report was exported successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to export the report")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<ExportResult>> ExportInvoicesReportAsync([FromQuery][Required] Guid clientId, [FromQuery][Required] ExportFormat format,
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await sender.Send(new InvoicesReportRequest(clientId, format, startDate, endDate));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Export)]
    [HttpGet("items")]
    [SwaggerOperation(Summary = "Export items report", Description = "Generates and exports a report of client items in required format using the provided client id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Report was exported successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to export the report")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<ExportResult>> ExportItemsReportAsync([FromQuery][Required] Guid clientId, [FromQuery][Required] ExportFormat format,
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
{
        var result = await sender.Send(new ItemsReportRequest(clientId, format, startDate, endDate));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Export)]
    [HttpGet("plans")]
    [SwaggerOperation(Summary = "Export plans report", Description = "Generates and exports a report of client plans in required format using the provided client id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Report was exported successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to export the report")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<ExportResult>> ExportPlansReportAsync([FromQuery][Required] Guid clientId, [FromQuery][Required] ExportFormat format,
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var result = await sender.Send(new PlansReportRequest(clientId, format, startDate, endDate));
        return FromResult(result);
    }

    [NonAction]
    public async Task SendReportsAsync()
    {
        await sender.Send(new SendReportsRequest());
    }
}
