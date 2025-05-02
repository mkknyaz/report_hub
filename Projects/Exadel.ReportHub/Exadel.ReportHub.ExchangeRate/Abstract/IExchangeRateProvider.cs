using Exadel.ReportHub.Data.Models;

namespace Exadel.ReportHub.Ecb.Abstract;

public interface IExchangeRateProvider
{
    Task<ExchangeRate> GetByCurrencyForWeekAsync(string currency, DateTime weekEndDate, CancellationToken cancellationToken);
}
