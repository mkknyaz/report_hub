namespace Exadel.ReportHub.Ecb;

public interface IExchangeRateProvider
{
    Task<IList<Data.Models.ExchangeRate>> GetDailyRatesAsync(CancellationToken cancellationToken);
}
