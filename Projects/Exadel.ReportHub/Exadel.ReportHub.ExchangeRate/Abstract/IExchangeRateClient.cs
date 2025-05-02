using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.Ecb.Abstract;

public interface IExchangeRateClient
{
    Task<IList<ExchangeRate>> GetByCurrencyInPeriodAsync(string currency, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
