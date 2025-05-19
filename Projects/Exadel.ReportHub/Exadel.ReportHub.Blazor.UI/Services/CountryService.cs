using Exadel.ReportHub.Blazor.UI.Services.Abstractions;
using Exadel.ReportHub.SDK.DTOs.Country;
using Exadel.ReportHub.SDK.DTOs.Currency;

namespace Exadel.ReportHub.Blazor.UI.Services;

public class CountryService : ICountryService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CountryService(IHttpClientFactory httpClientFactory)
        => _httpClientFactory = httpClientFactory;

    public async Task<IList<CountryDTO>> GetCountriesAsync()
    {
        var client = _httpClientFactory.CreateClient("api");
        var list = await client.GetFromJsonAsync<IList<CountryDTO>>("api/countries");
        return list ?? Array.Empty<CountryDTO>();
    }

    public async Task<IList<CurrencyDTO>> GetCurrenciesAsync()
    {
        var countries = await GetCountriesAsync();

        var distinct = countries
          .GroupBy(c => c.CurrencyId)
          .Select(g => g.First())
          .Select(c => new CurrencyDTO
          {
              CurrencyId = c.CurrencyId,
              CurrencyCode = c.CurrencyCode
          })
          .ToList();

        return distinct;
    }
}
