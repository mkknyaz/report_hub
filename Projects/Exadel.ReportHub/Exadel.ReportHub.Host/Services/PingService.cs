using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/ping")]
public class PingService : BaseService
{
    [HttpGet]
    public IActionResult Ping()
    {
        return Ok();
    }
}
