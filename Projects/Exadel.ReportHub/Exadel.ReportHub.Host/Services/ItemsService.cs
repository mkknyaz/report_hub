using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Exadel.ReportHub.Handlers.Item.Create;
using Exadel.ReportHub.Handlers.Item.Delete;
using Exadel.ReportHub.Handlers.Item.GetByClientId;
using Exadel.ReportHub.Handlers.Item.GetById;
using Exadel.ReportHub.Handlers.Item.Update;
using Exadel.ReportHub.Host.Services.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

[ExcludeFromCodeCoverage]
[Route("api/items")]
public class ItemsService(ISender sender) : BaseService
{
    [Authorize(Policy = Constants.Authorization.Policy.Create)]
    [HttpPost]
    public async Task<ActionResult<ItemDTO>> AddItem([FromBody] CreateUpdateItemDTO createItemDto)
    {
        var result = await sender.Send(new CreateItemRequest(createItemDto));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet]
    public async Task<ActionResult<IList<ItemDTO>>> GetItemsByClientId([FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetItemsByClientIdRequest(clientId));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Read)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ItemDTO>> GetItemById([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new GetItemByIdRequest(id));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Update)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateItem([FromRoute] Guid id, [FromBody] CreateUpdateItemDTO updateItemDTO)
    {
        var result = await sender.Send(new UpdateItemRequest(id, updateItemDTO));

        return FromResult(result);
    }

    [Authorize(Policy = Constants.Authorization.Policy.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteItem([FromRoute] Guid id, [FromQuery][Required] Guid clientId)
    {
        var result = await sender.Send(new DeleteItemRequest(id));

        return FromResult(result);
    }
}
