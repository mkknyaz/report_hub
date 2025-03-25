using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ApiController]
[Route("api/[controller]")]
public class TestService : ControllerBase
{
    [HttpGet]
    public IActionResult GetSampleAnswer()
    {
        return Ok();
    }
}
