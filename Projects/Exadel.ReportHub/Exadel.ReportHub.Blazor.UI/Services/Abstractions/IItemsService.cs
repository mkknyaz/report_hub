using Exadel.ReportHub.SDK.DTOs.Item;

namespace Exadel.ReportHub.Blazor.UI.Services.Abstractions;
public interface IItemsService
{
    Task<IList<ItemDTO>> GetItemsByClientIdAsync(Guid clientId);

    Task<ItemDTO> GetItemByIdAsync(Guid itemId);

    Task CreateItemAsync(CreateUpdateItemDTO dto);

    Task UpdateItemAsync(Guid itemId, CreateUpdateItemDTO dto);

    Task DeleteItemAsync(Guid itemId);
}
