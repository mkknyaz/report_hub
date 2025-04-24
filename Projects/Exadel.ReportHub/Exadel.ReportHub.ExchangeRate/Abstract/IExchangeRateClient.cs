using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.Ecb.Abstract;

public interface IExchangeRateClient
{
    Task<IList<ExchangeRate>> GetDailyRatesAsync(CancellationToken cancellationToken);

    Task<ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken);
}
