using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.RA.Abstract;
using MongoDB.Driver;

namespace Exadel.ReportHub.Ecb;

public class ExchangeRateProvider(IExchangeRateRepository exhangeRateRepository, IExchangeRateClient exchangeRateClient) : IExchangeRateClient
{
    public async Task<Data.Models.ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken)
    {
        var exchangeRate = await exhangeRateRepository.GetByCurrencyAsync(currency, cancellationToken);

        if (exchangeRate != null)
        {
            return exchangeRate;
        }

        var exchangeRates = await GetRatesAsync(cancellationToken);
        return exchangeRates.SingleOrDefault(x => x.Currency.Equals(currency, StringComparison.Ordinal));
    }

    public async Task<IList<Data.Models.ExchangeRate>> GetDailyRatesAsync(CancellationToken cancellationToken)
    {
        var exchangeRates = await exhangeRateRepository.GetAllAsync(cancellationToken);

        if (exchangeRates.Any())
        {
            return exchangeRates;
        }

        return await GetRatesAsync(cancellationToken);
    }

    private async Task<IList<Data.Models.ExchangeRate>> GetRatesAsync(CancellationToken cancellationToken)
    {
        var exchangeRates = await exchangeRateClient.GetDailyRatesAsync(cancellationToken);

        if (exchangeRates.Any())
        {
            await exhangeRateRepository.AddManyAsync(exchangeRates, cancellationToken);
        }

        return exchangeRates;
    }
}
