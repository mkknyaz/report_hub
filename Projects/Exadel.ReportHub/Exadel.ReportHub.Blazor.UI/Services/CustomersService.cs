using System.Net.Http;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Exadel.ReportHub.SDK.DTOs.Customer;
using static System.Net.WebRequestMethods;

namespace Exadel.ReportHub.Blazor.UI.Services;

public class CustomersService : ICustomersService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserStateService _userStateService;

    public CustomersService(IHttpClientFactory httpClientFactory, IUserStateService userStateService)
    {
        _httpClientFactory = httpClientFactory;
        _userStateService = userStateService;
    }

    public async Task<IList<CustomerDTO>> GetCustomersByClientIdAsync(Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");

        return await client.GetFromJsonAsync<IList<CustomerDTO>>($"api/customers?clientId={clientId}") ?? [];
    }

    public async Task<CustomerDTO> GetCustomerByIdAsync(Guid customerId, Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");

        return await client.GetFromJsonAsync<CustomerDTO>($"api/customers/{customerId}?clientId={clientId}");
    }

    public async Task<CustomerDTO> CreateCustomerAsync(CreateCustomerDTO dto)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.PostAsJsonAsync("api/customers", dto);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CustomerDTO>();
    }

    public async Task UpdateCustomerAsync(Guid customerId, UpdateCustomerDTO dto, Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.PutAsJsonAsync(
            $"api/customers/{customerId}?clientId={clientId}", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCustomerAsync(Guid customerId, Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.DeleteAsync($"api/customers/{customerId}?clientId={clientId}");
        response.EnsureSuccessStatusCode();
    }
}
