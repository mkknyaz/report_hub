using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using static System.Net.WebRequestMethods;

namespace Exadel.ReportHub.Blazor.UI.Services;

public class InvoicesService : IInvoicesService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public InvoicesService(IHttpClientFactory httpClientFactory, IUserStateService userStateService)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task CreateInvoiceAsync(CreateInvoiceDTO dto)
    {
        var client = _httpClientFactory.CreateClient("api");
        var url = $"api/invoices?clientId={dto.ClientId}";
        var response = await client.PostAsJsonAsync(url, dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteInvoiceAsync(Guid invoiceId, Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var url = $"api/invoices/{invoiceId}?clientId={clientId}";
        var response = await client.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }

    public async Task<InvoiceDTO> GetInvoiceByIdAsync(Guid invoiceId, Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var url = $"api/invoices/{invoiceId}?clientId={clientId}";
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
                {
                    new JsonStringEnumConverter(
                        JsonNamingPolicy.CamelCase,
                        allowIntegerValues: true)
                }
        };


        return await client.GetFromJsonAsync<InvoiceDTO>(url, options);
    }

    public async Task<IList<InvoiceDTO>> GetInvoicesByClientIdAsync(Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var url = $"api/invoices?clientId={clientId}";
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
                {
                    new JsonStringEnumConverter(
                        JsonNamingPolicy.CamelCase,
                        allowIntegerValues: true)
                }
        };

        return await client.GetFromJsonAsync<IList<InvoiceDTO>>(url, options);
    }

    public async Task UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceDTO dto, Guid clientId)
    {
        var client = _httpClientFactory.CreateClient("api");
        var url = $"api/invoices/{invoiceId}?clientId={clientId}";
        var response = await client.PutAsJsonAsync(url, dto);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
                {
                    new JsonStringEnumConverter(
                        JsonNamingPolicy.CamelCase,
                        allowIntegerValues: true)
                }
        };

        response.EnsureSuccessStatusCode();
    }
}
