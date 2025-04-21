using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Customer.Create;
using Exadel.ReportHub.Handlers.Customer.Delete;
using Exadel.ReportHub.Handlers.Customer.Get;
using Exadel.ReportHub.Handlers.Customer.GetById;
using Exadel.ReportHub.Handlers.Customer.Update;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/customers")]
public class CustomersService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    public async Task<ActionResult<CustomerDTO>> AddCustomer([FromBody] CreateCustomerDTO createCustomerDto)
    {
        var result = await sender.Send(new CreateCustomerRequest(createCustomerDto));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    public async Task<ActionResult<IList<CustomerDTO>>> GetCustomers()
    {
        var result = await sender.Send(new GetCustomersRequest());
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDTO>> GetCustomerById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetCustomerByIdRequest(id));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateCustomer([FromRoute] Guid id, [FromBody] UpdateCustomerDTO updateCustomerDTO)
    {
        var result = await sender.Send(new UpdateCustomerRequest(id, updateCustomerDTO));
        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteCustomer([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteCustomerRequest(id));
        return FromResult(result);
    }
}
