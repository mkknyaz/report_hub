using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Handlers.Invoice.Create;
using Exadel.ReportHub.Handlers.Invoice.Delete;
using Exadel.ReportHub.Handlers.Invoice.ExportPdf;
using Exadel.ReportHub.Handlers.Invoice.GetByClientId;
using Exadel.ReportHub.Handlers.Invoice.GetById;
using Exadel.ReportHub.Handlers.Invoice.GetByOverdueStatus;
using Exadel.ReportHub.Handlers.Invoice.GetCount;
using Exadel.ReportHub.Handlers.Invoice.GetRevenue;
using Exadel.ReportHub.Handlers.Invoice.Import;
using Exadel.ReportHub.Handlers.Invoice.Update;
using Exadel.ReportHub.Handlers.Invoice.UpdateOverdueStatus;
using Exadel.ReportHub.Handlers.Invoice.UpdatePaidStatus;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.Abstract;
using Exadel.ReportHub.SDK.DTOs.Import;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/invoices")]
public class InvoicesService(ISender sender) : BaseService, IInvoiceService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost("import")]
    [SwaggerOperation(Summary = "Import invoices", Description = "Imports invoices from the provided form data")]
    [SwaggerResponse(StatusCodes.Status201Created, "Invoices were imported successfully", typeof(ActionResult<ImportResultDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation errors occurred during import", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to perform this action")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<ImportResultDTO>> ImportInvoicesAsync([FromForm] ImportDTO importDto, [FromQuery, Required] Guid clientId)
    {
        var result = await sender.Send(new ImportInvoicesRequest(importDto));
        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new invoice", Description = "Creates a new invoice and returns its details")]
    [SwaggerResponse(StatusCodes.Status201Created, "Invoice was created successfully", typeof(ActionResult<InvoiceDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The request contains invalid invoice data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to create invoices")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<InvoiceDTO>> AddInvoice([FromBody] CreateInvoiceDTO invoiceDto)
    {
        var result = await sender.Send(new CreateInvoiceRequest(invoiceDto));
        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get invoices by client id", Description = "Returns a list of invoices for the specified client")]
    [SwaggerResponse(StatusCodes.Status200OK, "Invoices were retrieved successfully", typeof(ActionResult<IList<InvoiceDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this invoice")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<IList<InvoiceDTO>>> GetInvoicesByClientId([FromQuery, Required] Guid clientId)
    {
        var result = await sender.Send(new GetInvoicesByClientIdRequest(clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get invoice by id", Description = "Returns the invoice details for the specified id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Invoice was retrieved successfully", typeof(ActionResult<InvoiceDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this invoice")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Invoice was not found for the given id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<InvoiceDTO>> GetInvoiceById([FromRoute] Guid id, [FromQuery, Required] Guid clientId)
    {
        var result = await sender.Send(new GetInvoiceByIdRequest(id));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete invoice", Description = "Deletes the invoice with the specified id")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Invoice was deleted successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The request is invalid", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to delete the invoice")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Invoice was not found for the specified id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> DeleteInvoice([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeleteInvoiceRequest(id, clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update invoice", Description = "Updates the invoice with the specified id")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Invoice was updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The request contains invalid invoice data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this invoice")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Invoice was not found for the specified id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateInvoice([FromRoute] Guid id, [FromQuery] Guid clientId, [FromBody] UpdateInvoiceDTO invoiceDto)
    {
        var result = await sender.Send(new UpdateInvoiceRequest(id, clientId, invoiceDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Export)]
    [HttpGet("{id:guid}/export")]
    [SwaggerOperation(Summary = "Export invoice as PDF", Description = "Generates and exports a PDF version of the invoice using the provided invoice id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Invoices were exported successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this invoice")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Invoice was not found for the specified id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<ExportResult>> ExportInvoiceAsync(Guid id, Guid clientId)
    {
        var result = await sender.Send(new ExportPdfInvoiceRequest(id));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("revenue")]
    [SwaggerOperation(Summary = "Get total revenue", Description = "Returns the total revenue for the specified client")]
    [SwaggerResponse(StatusCodes.Status200OK, "Total revenue was retrieved successfully", typeof(ActionResult<TotalInvoicesRevenueDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this invoice")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Invoice was not found for the specified dates", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<TotalInvoicesRevenueDTO>> GetRevenue([FromQuery] InvoiceRevenueFilterDTO invoiceRevenueFilterDto)
    {
        var result = await sender.Send(new GetInvoicesRevenueRequest(invoiceRevenueFilterDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("count")]
    [SwaggerOperation(Summary = "Get total number of invoices within specified date range", Description = "Returns the total number of invoices for specific client/customer")]
    [SwaggerResponse(StatusCodes.Status200OK, "Invoices number was retrieved successfully", typeof(ActionResult<InvoiceCountResultDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this invoice")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<InvoiceCountResultDTO>> GetCount([FromQuery] InvoiceCountFilterDTO invoiceCountFilterDto)
    {
        var result = await sender.Send(new GetInvoiceCountRequest(invoiceCountFilterDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPatch("{id:guid}/pay")]
    [SwaggerOperation(Summary = "Mark invoice as paid", Description = "Marks the invoice as paid — on time or late depending on due date")]
    [SwaggerResponse(StatusCodes.Status200OK, "Invoice payment status was updated successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to update this invoice")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Invoice was not found for the specified id", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> PayInvoice([FromRoute] Guid id, [FromQuery, Required] Guid clientId)
    {
        var result = await sender.Send(new UpdateInvoicePaidStatusRequest(id, clientId));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("overdue")]
    [SwaggerOperation(Summary = "Get overdue invoices summary", Description = "Returns the total number of overdue invoices and their amount for the specified client")]
    [SwaggerResponse(StatusCodes.Status200OK, "Overdue invoices data was retrieved successfully", typeof(ActionResult<OverdueInvoicesResultDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have permission to access this invoice")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Invoice was not found for the specified dates", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<OverdueInvoicesResultDTO>> GetOverdue([Required] Guid clientId)
    {
        var result = await sender.Send(new GetInvoicesByOverdueStatusRequest(clientId));
        return FromResult(result);
    }

    [NonAction]
    public async Task UpdateOverdueInvoicesStatusAsync()
    {
        await sender.Send(new UpdateOverdueInvoicesStatusRequest());
    }
}
