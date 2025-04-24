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
    [SwaggerResponse(StatusCodes.Status201Created, "Item was created successfully", typeof(ActionResult<ItemDTO>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to create an item")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<ItemDTO>> AddItem([FromBody] CreateUpdateItemDTO createItemDto)
    {
        var result = await sender.Send(new CreateItemRequest(createItemDto));

        return FromResult(result, StatusCodes.Status201Created);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get items by client id", Description = "Returns a list of items for the specified client.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Items were retrieved successfully", typeof(ActionResult<IList<ItemDTO>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to access the items")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<IList<ItemDTO>>> GetItemsByClientId([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetItemsByClientIdRequest(clientId));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    [SwaggerOperation(Summary = "Get item by id", Description = "Returns the details of the specified item for the given client.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Item was retrieved successfully", typeof(ActionResult<ItemDTO>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "User doesnt have permission to access this item")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Item was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult<ItemDTO>> GetItemById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetItemByIdRequest(id));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Update item", Description = "Updates the item with the specified id.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Item was updated successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request data", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Users doesnt have permission to update this item")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Item was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> UpdateItem([FromRoute] Guid id, [FromBody] CreateUpdateItemDTO updateItemDTO)
    {
        var result = await sender.Send(new UpdateItemRequest(id, updateItemDTO));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Delete item", Description = "Deletes the item with the specified id and client.")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Item was deleted successfully")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authentication is required to access this endpoint")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Users doesnt have permission to delete this item")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Item was not found", typeof(ErrorResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, type: typeof(ErrorResponse))]
    public async Task<ActionResult> DeleteItem([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeleteItemRequest(id));

        return FromResult(result);
    }
}
