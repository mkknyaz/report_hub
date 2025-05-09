using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.TestEmail;
using Exadel.ReportHub.Host.Services.Abstract;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/test")]
public class TestService(ISender sender) : BaseService
{
    [HttpGet("send-email")]
    public async Task<ActionResult<TestEmailResult>> SendEmail([FromQuery] Guid invoiceId, [FromQuery] string email, [FromQuery] string subject)
    {
        var result = await sender.Send(new TestEmailRequest(invoiceId, email, subject));

        return FromResult(result);
    }
}
