using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Exadel.ReportHub.SDK.DTOs.Item;
using Exadel.ReportHub.SDK.DTOs.User;
using static System.Net.WebRequestMethods;

namespace Exadel.ReportHub.Blazor.UI.Services;

public class ItemsService : IItemsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserStateService _userStateService;

    public ItemsService(IHttpClientFactory httpClientFactory, IUserStateService userStateService)
    {
        _httpClientFactory = httpClientFactory;
        _userStateService = userStateService;
      _userStateService = userStateService;
    }

    public async Task<IList<ItemDTO>> GetItemsByClientIdAsync(Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.GetAsync($"api/items?clientId={clientId}");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

        var data = await response.Content.ReadFromJsonAsync<IList<ItemDTO>>();
        return data ?? new List<ItemDTO>();
    }

    public async Task DeleteItemAsync(Guid itemId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var clientId = _userStateService.GetSelectedClientId();

        var response = await client.DeleteAsync($"api/items/{itemId}?clientId={clientId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException(
              $"Failed to delete item {itemId}. " +
              $"Status code: {response.StatusCode}");
        }
    }

    public async Task<ItemDTO> GetItemByIdAsync(Guid itemId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var clientId = _userStateService.GetSelectedClientId();

        return await client.GetFromJsonAsync<ItemDTO>($"api/items/{itemId}?clientId={clientId}");
    }

    public async Task CreateItemAsync(CreateUpdateItemDTO dto)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.PostAsJsonAsync("api/items", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateItemAsync(Guid itemId, CreateUpdateItemDTO dto)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.PutAsJsonAsync($"api/items/{itemId}", dto);
        response.EnsureSuccessStatusCode();
    }
}
