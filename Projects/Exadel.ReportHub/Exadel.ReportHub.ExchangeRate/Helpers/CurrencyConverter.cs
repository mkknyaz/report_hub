using Exadel.ReportHub.Ecb.Abstract;

namespace Exadel.ReportHub.Ecb.Helpers;

public class CurrencyConverter : ICurrencyConverter
{
    private readonly IExchangeRateProvider _exchangeRateProvider;

    public CurrencyConverter(IExchangeRateProvider exchangeRateProvider)
    {
        _exchangeRateProvider = exchangeRateProvider;
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, DateTime date, CancellationToken cancellationToken)
    {
        if (fromCurrency.Equals(toCurrency, StringComparison.Ordinal))
        {
            return amount;
        }

        decimal fromRate = 1;
        if (!fromCurrency.Equals(Constants.Currency.DefaultCurrencyCode, StringComparison.Ordinal))
        {
            fromRate = (await _exchangeRateProvider.GetByCurrencyForWeekAsync(fromCurrency, date, cancellationToken)).Rate;
        }

        decimal toRate = 1;
        if (!toCurrency.Equals(Constants.Currency.DefaultCurrencyCode, StringComparison.Ordinal))
        {
            toRate = (await _exchangeRateProvider.GetByCurrencyForWeekAsync(toCurrency, date, cancellationToken)).Rate;
        }

        return amount * toRate / fromRate;
    }
}
