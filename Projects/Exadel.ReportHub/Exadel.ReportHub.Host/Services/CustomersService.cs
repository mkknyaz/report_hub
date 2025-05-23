﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Customer.Create;
using Exadel.ReportHub.Handlers.Customer.Delete;
using Exadel.ReportHub.Handlers.Customer.Get;
using Exadel.ReportHub.Handlers.Customer.GetById;
using Exadel.ReportHub.Handlers.Customer.Import;
using Exadel.ReportHub.Handlers.Customer.Update;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.SDK.DTOs.Import;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/customers")]
public class CustomersService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost("import")]
    [SwaggerOperation(Summary = "Import customers", Description = "Imports a list of customers from a file.")]
    [SwaggerResponse(StatusCodes.Status201Created, Constants.SwaggerSummary.Customer.Status201ImportDescription, typeof(ActionResult<ImportResultDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ImportResultDTO>> ImportCustomers([FromForm] ImportDTO importDto, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new ImportCustomersRequest(clientId, importDto));
        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new customer", Description = "Creates a new customer and returns its details.")]
    [SwaggerResponse(StatusCodes.Status201Created, Constants.SwaggerSummary.Customer.Status201CreateDescription, typeof(ActionResult<CustomerDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<CustomerDTO>> AddCustomer([FromBody] CreateCustomerDTO createCustomerDto)
    {
        var result = await sender.Send(new CreateCustomerRequest(createCustomerDto));

        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get customers by client id", Description = "Returns a list of all available customers.")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Customer.Status200RetrieveDescription, typeof(ActionResult<IList<CustomerDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<IList<CustomerDTO>>> GetCustomersByClient([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetCustomersByClientIdRequest(clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get customer by id", Description = "Returns the customer details for the specified id.")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Customer.Status200RetrieveDescription, typeof(ActionResult<CustomerDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Customer.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<CustomerDTO>> GetCustomerById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetCustomerByIdRequest(id, clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update customer", Description = "Updates the customer information with the specified id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.Customer.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Customer.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateCustomer([FromRoute] Guid id, [FromQuery][Required] Guid clientId, [FromBody] UpdateCustomerDTO updateCustomerDto)
    {
        var result = await sender.Send(new UpdateCustomerRequest(id, clientId, updateCustomerDto));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete customer", Description = "Deletes the customer with the specified id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.Customer.Status204DeleteDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Customer.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> DeleteCustomer([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeleteCustomerRequest(id, clientId));
        return FromResult(result);
    }
}
