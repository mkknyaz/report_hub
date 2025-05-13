using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Item.Create;
using Exadel.ReportHub.Handlers.Item.Delete;
using Exadel.ReportHub.Handlers.Item.GetByClientId;
using Exadel.ReportHub.Handlers.Item.GetById;
using Exadel.ReportHub.Handlers.Item.Update;
using Exadel.ReportHub.Host.Infrastructure.Models;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/items")]
public class ItemsService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new item", Description = "Creates a new item and returns its details.")]
    [SwaggerResponse(StatusCodes.Status201Created, Constants.SwaggerSummary.Item.Status201CreateDescription, typeof(ActionResult<ItemDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ItemDTO>> AddItem([FromBody] CreateUpdateItemDTO createItemDto)
    {
        var result = await sender.Send(new CreateItemRequest(createItemDto));

        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get items by client id", Description = "Returns a list of items for the specified client.")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Item.Status200RetrieveDescription, typeof(ActionResult<IList<ItemDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<IList<ItemDTO>>> GetItemsByClientId([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetItemsByClientIdRequest(clientId));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get item by id", Description = "Returns the details of the specified item for the given client.")]
    [SwaggerResponse(StatusCodes.Status200OK, Constants.SwaggerSummary.Item.Status200RetrieveDescription, typeof(ActionResult<ItemDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Item.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult<ItemDTO>> GetItemById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetItemByIdRequest(id));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update item", Description = "Updates the item with the specified id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.Item.Status204UpdateDescription)]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Constants.SwaggerSummary.Common.Status400Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Item.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateItem([FromRoute] Guid id, [FromBody] CreateUpdateItemDTO updateItemDto)
    {
        var result = await sender.Send(new UpdateItemRequest(id, updateItemDto));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete item", Description = "Deletes the item with the specified id and client.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, Constants.SwaggerSummary.Item.Status204DeleteDescription)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, Constants.SwaggerSummary.Common.Status401Description)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, Constants.SwaggerSummary.Common.Status403Description)]
    [SwaggerResponse(StatusCodes.Status404NotFound, Constants.SwaggerSummary.Item.Status404Description, typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Constants.SwaggerSummary.Common.Status500Description, typeof(ErrorResponse))]
    public async Task<ActionResult> DeleteItem([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeleteItemRequest(id));

        return FromResult(result);
    }
}
