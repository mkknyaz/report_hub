using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Report.Invoices;
using Exadel.ReportHub.Handlers.Report.Items;
using Exadel.ReportHub.Handlers.Report.Plans;
using Exadel.ReportHub.Handlers.Report.Send;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;
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
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Report.Status200ExportDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ExportResult>> ExportInvoicesReportAsync([FromQuery] ExportReportDTO exportReportDto)
    {
        var result = await sender.Send(new InvoicesReportRequest(exportReportDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Export)]
    [HttpGet("items")]
    [SwaggerOperation(Summary = "Export items report", Description = "Generates and exports a report of client items in required format using the provided client id")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Report.Status200ExportDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ExportResult>> ExportItemsReportAsync([FromQuery] ExportReportDTO exportReportDto)
{
        var result = await sender.Send(new ItemsReportRequest(exportReportDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Export)]
    [HttpGet("plans")]
    [SwaggerOperation(Summary = "Export plans report", Description = "Generates and exports a report of client plans in required format using the provided client id")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Report.Status200ExportDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ExportResult>> ExportPlansReportAsync([FromQuery] ExportReportDTO exportReportDto)
    {
        var result = await sender.Send(new PlansReportRequest(exportReportDto));
        return FromResult(result);
    }

    [NonAction]
    public async Task SendReportsAsync()
    {
        await sender.Send(new SendReportsRequest());
    }
}
