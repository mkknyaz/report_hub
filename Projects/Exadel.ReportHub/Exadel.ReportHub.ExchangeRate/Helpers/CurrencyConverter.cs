using Exadel.ReportHub.Ecb.Abstract;

namespace Exadel.ReportHub.Ecb.Helpers;

public class CurrencyConverter : ICurrencyConverter
{
    private readonly IExchangeRateClient _exchangeRateProvider;

    public CurrencyConverter(IExchangeRateClient exchangeRateProvider)
    {
        _exchangeRateProvider = exchangeRateProvider;
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken)
    {
        if (fromCurrency.Equals(toCurrency, StringComparison.Ordinal))
        {
            return amount;
        }

        decimal fromRate = 1;
        if (!fromCurrency.Equals(Constants.Currency.DefaultCurrencyCode, StringComparison.Ordinal))
        {
            fromRate = (await _exchangeRateProvider.GetByCurrencyAsync(fromCurrency, cancellationToken)).Rate;
        }

        decimal toRate = 1;
        if (!toCurrency.Equals(Constants.Currency.DefaultCurrencyCode, StringComparison.Ordinal))
        {
            toRate = (await _exchangeRateProvider.GetByCurrencyAsync(toCurrency, cancellationToken)).Rate;
        }

        return amount * toRate / fromRate;
    }
}
