using System.ComponentModel.DataAnnotations;
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
    [SwaggerResponse(StatusCodes.Status201Created, "Customers were imported successfully", typeof(ActionResult<ImportResultDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to import a Customer")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ImportResultDTO>> ImportCustomers([FromForm] ImportDTO importDto, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new ImportCustomersRequest(importDto));
        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new customer", Description = "Creates a new customer and returns its details.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Customer was created successfully", typeof(ActionResult<CustomerDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to add a Customer")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerDTO>> AddCustomer([FromBody] CreateCustomerDTO createCustomerDto)
    {
        var result = await sender.Send(new CreateCustomerRequest(createCustomerDto));

        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get customers by client id", Description = "Returns a list of all available customers.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Customers were retrieved successfully", typeof(ActionResult<IList<CustomerDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to access a Customers")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IList<CustomerDTO>>> GetCustomersByClient([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetCustomersRequest(clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get customer by id", Description = "Returns the customer details for the specified id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Customer was retrieved successfully", typeof(ActionResult<CustomerDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to access a Customer")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<CustomerDTO>> GetCustomerById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetCustomerByIdRequest(id, clientId));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update customer", Description = "Updates the customer information with the specified id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Customer was updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to update a Customer")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateCustomer([FromRoute] Guid id, [FromQuery][Required] Guid clientId, [FromBody] UpdateCustomerDTO updateCustomerDTO)
    {
        var result = await sender.Send(new UpdateCustomerRequest(id, clientId, updateCustomerDTO));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete customer", Description = "Deletes the customer with the specified id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Customer was deleted successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to delete a Customer")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Customer was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> DeleteCustomer([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeleteCustomerRequest(id, clientId));
        return FromResult(result);
    }
}
