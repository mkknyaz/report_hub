using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Host.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/ping")]
public class PingService : BaseService
{
    [HttpGet]
    public ActionResult Ping()
    {
        return Ok();
    }
}
