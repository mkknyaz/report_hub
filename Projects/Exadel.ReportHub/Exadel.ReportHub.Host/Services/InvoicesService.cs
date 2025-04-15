using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Invoice.Import;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/invoices")]
public class InvoicesService(ISender sender) : BaseService
{
    [HttpPost("import")]
    public async Task<IActionResult> ImportInvoicesAsync([FromForm] ImportDTO importDto)
    {
        var result = await sender.Send(new ImportInvoicesRequest(importDto));

        return FromResult(result);
    }
}
