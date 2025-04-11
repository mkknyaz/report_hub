using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

public class ClientService : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.ClientAdmin)]
    [HttpGet("{id:guid}")]
    public IActionResult ClientAdminTest([FromRoute] Guid id)
    {
        return Ok();
    }
}
