using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Client.Create;
using Exadel.ReportHub.Handlers.Client.Delete;
using Exadel.ReportHub.Handlers.Client.Get;
using Exadel.ReportHub.Handlers.Client.GetById;
using Exadel.ReportHub.Handlers.Client.UpdateName;
using Exadel.ReportHub.SDK.DTOs.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/clients")]
public class ClientsService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> AddClient([FromBody] CreateClientDTO createClientDto)
    {
        var result = await sender.Send(new CreateClientRequest(createClientDto));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.SuperAdmin)]
    [Authorize(Policy = Constants.Authorization.Policy.Owner)]
    [Authorize(Policy = Constants.Authorization.Policy.ClientAdmin)]
    [Authorize(Policy = Constants.Authorization.Policy.Operator)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClientById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetClientByIdRequest(id));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.SuperAdmin)]
    [Authorize(Policy = Constants.Authorization.Policy.Owner)]
    [Authorize(Policy = Constants.Authorization.Policy.ClientAdmin)]
    [Authorize(Policy = Constants.Authorization.Policy.Operator)]
    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var result = await sender.Send(new GetClientsRequest());

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.SuperAdmin)]
    [Authorize(Policy = Constants.Authorization.Policy.Owner)]
    [HttpPatch("{id:guid}/name")]
    public async Task<IActionResult> UpdateClientName([FromRoute] Guid id, [FromBody] string name)
    {
        var result = await sender.Send(new UpdateClientNameRequest(id, name));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.SuperAdmin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClient([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteClientRequest(id));

        return FromResult(result);
    }
}
