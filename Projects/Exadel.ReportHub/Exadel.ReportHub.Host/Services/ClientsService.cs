using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Client.Create;
using Exadel.ReportHub.Handlers.Client.Delete;
using Exadel.ReportHub.Handlers.Client.Get;
using Exadel.ReportHub.Handlers.Client.GetById;
using Exadel.ReportHub.Handlers.Client.Import;
using Exadel.ReportHub.Handlers.Client.UpdateName;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.Import;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/clients")]
public class ClientsService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost("import")]
    [SwaggerOperation(Summary = "Import clients", Description = "Imports a list of clients from a file and returns the result of the import process")]
    [SwaggerResponse(StatusCodes.Status201Created, Constants.SwaggerSummary.Client.Status201ImportDescription, typeof(ActionResult<ImportResultDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ImportResultDTO>> ImportClients([FromForm] ImportDTO importDto, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new ImportClientRequest(importDto));
        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new client", Description = "Creates a new client and returns the created client object")]
    [SwaggerResponse(StatusCodes.Status201Created, Constants.SwaggerSummary.Client.Status201CreateDescription, typeof(ActionResult<ClientDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ClientDTO>> AddClient([FromBody] CreateClientDTO createClientDto)
    {
        var result = await sender.Send(new CreateClientRequest(createClientDto));

        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get client by id", Description = "Retrieves a specific client using their unique identifier")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Client.Status200RetrieveDescription, typeof(ActionResult<ClientDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Client.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ClientDTO>> GetClientById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetClientByIdRequest(id));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get all clients", Description = "Retrieves a list of all registered clients")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Client.Status200RetrieveDescription, typeof(ActionResult<IList<ClientDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<IList<ClientDTO>>> GetClients()
    {
        var result = await sender.Send(new GetClientsRequest());

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPatch("{id:guid}/name")]
    [SwaggerOperation(Summary = "Update client name", Description = "Updates the name of an existing client by id")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.Client.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Client.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateClientName([FromRoute] Guid id, [FromBody] string name)
    {
        var result = await sender.Send(new UpdateClientNameRequest(id, name));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete client", Description = "Deletes a specific client by id")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.Client.Status204DeleteDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Client.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> DeleteClient([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteClientRequest(id));

        return FromResult(result);
    }
}
