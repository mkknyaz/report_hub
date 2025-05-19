using System.Text.Json.Serialization;
using System.Text.Json;
using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Exadel.ReportHub.SDK.DTOs.User;

namespace Exadel.ReportHub.Blazor.UI.Services;

public class ClientsService : IClientsService
{
	private readonly IHttpClientFactory _httpClientFactory;

	public ClientsService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task<IList<UserClientDTO>> GetClientsAsync()
	{
        var client = _httpClientFactory.CreateClient("api");
        var response = await client.GetAsync("api/users/clients");

        if (!response.IsSuccessStatusCode)
        {
            return [];
        }

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

        var clients = await response.Content.ReadFromJsonAsync<IList<UserClientDTO>>(options);

        return clients ?? [];
    }
}