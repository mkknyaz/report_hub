using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/ping")]
public class PingService : BaseService
{
    [HttpGet]
    [SwaggerOperation(Summary = "Ping server", Description = "Checks the server's health")]
    [SwaggerResponse(StatusCodes.Status200OK, "Server is healthy")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public ActionResult Ping()
    {
        return Ok();
    }
}
