using Exadel.ReportHub.SDK.DTOs.Country;
using Exadel.ReportHub.SDK.DTOs.Currency;

namespace Exadel.ReportHub.Blazor.UI.Services.Abstractions;

public interface ICountryService
{
    Task<IList<CountryDTO>> GetCountriesAsync();

    Task<IList<CurrencyDTO>> GetCurrenciesAsync();
}