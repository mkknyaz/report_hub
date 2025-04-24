using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.RA.Abstract;

public interface IExchangeRateRepository
{
    Task<IList<ExchangeRate>> GetAllAsync(CancellationToken cancellationToken);

    Task AddManyAsync(IEnumerable<ExchangeRate> exchangeRates, CancellationToken cancellationToken);

    Task<ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken);
}
