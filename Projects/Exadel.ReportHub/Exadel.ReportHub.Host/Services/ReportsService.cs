using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Report.Send;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.Abstract;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/reports")]
[ApiController]
public class ReportsService(ISender sender) : BaseService, IReportService
{
    [NonAction]
    public async Task SendReportsAsync()
    {
        await sender.Send(new SendReportsRequest());
    }
}
