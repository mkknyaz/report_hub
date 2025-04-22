using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Invoice.Create;
using Exadel.ReportHub.Handlers.Invoice.Delete;
using Exadel.ReportHub.Handlers.Invoice.GetByClientId;
using Exadel.ReportHub.Handlers.Invoice.GetById;
using Exadel.ReportHub.Handlers.Invoice.Import;
using Exadel.ReportHub.Handlers.Invoice.Update;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Import;
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
    public async Task<ActionResult<ImportResultDTO>> ImportInvoicesAsync([FromForm] ImportDTO importDto)
    {
        var result = await sender.Send(new ImportInvoicesRequest(importDto));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    public async Task<ActionResult<InvoiceDTO>> CreateInvoice([FromBody] CreateInvoiceDTO invoiceDto)
    {
        var result = await sender.Send(new CreateInvoiceRequest(invoiceDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    public async Task<ActionResult<IList<InvoiceDTO>>> GetInvoicesByClientId([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetInvoicesByClientIdRequest(clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceDTO>> GetInvoiceById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetInvoiceByIdRequest(id));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteInvoice([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeleteInvoiceRequest(id));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateInvoice([FromRoute] Guid id, [FromBody] UpdateInvoiceDTO invoiceDto)
    {
        var result = await sender.Send(new UpdateInvoiceRequest(id, invoiceDto));
        return FromResult(result);
    }
}
