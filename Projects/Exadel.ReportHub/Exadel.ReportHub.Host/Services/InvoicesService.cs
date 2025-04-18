using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Invoice.Import;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/invoices")]
public class InvoicesService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost("import")]
    public async Task<IActionResult> ImportInvoicesAsync([FromForm] ImportDTO importDto)
    {
        var result = await sender.Send(new ImportInvoicesRequest(importDto));

        return FromResult(result);
    }
}
