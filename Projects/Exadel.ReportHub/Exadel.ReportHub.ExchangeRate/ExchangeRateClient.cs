using System.Globalization;
using System.Xml.Linq;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Ecb.Abstract;
using Microsoft.Extensions.Logging;

namespace Exadel.ReportHub.Ecb;

public class ExchangeRateClient(IHttpClientFactory factory, ILogger<ExchangeRateClient> logger) : IExchangeRateClient
{
    public async Task<IList<ExchangeRate>> GetDailyRatesAsync(CancellationToken cancellationToken)
    {
        var client = factory.CreateClient(Constants.ClientName);

        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(Constants.Path.ExchangeRate, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(ex, Constants.Error.HttpFetchError);
            return new List<ExchangeRate>();
        }
        catch(TaskCanceledException ex)
        {
            logger.LogError(ex, Constants.Error.TimeoutError);
            return new List<ExchangeRate>();
        }

        var result = await response.Content.ReadAsStringAsync(cancellationToken);

        XDocument document;
        try
        {
            document = XDocument.Parse(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, Constants.Error.ParseError);
            return new List<ExchangeRate>();
        }

        var root = document.Root.GetDefaultNamespace();

        var cubeTime = document
            .Descendants(root + "Cube")
            .Single(x => x.Attribute("time") != null);

        var rateDate = DateTime.Parse(cubeTime.Attribute("time").Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).AddHours(16);

        var rates = cubeTime.Elements(root + "Cube")
            .Select(x => new ExchangeRate
            {
                Currency = x.Attribute("currency").Value,
                Rate = decimal.Parse(x.Attribute("rate").Value, CultureInfo.InvariantCulture),
                RateDate = rateDate
            })
            .ToList();

        return rates;
    }

    public async Task<ExchangeRate> GetByCurrencyAsync(string currency, CancellationToken cancellationToken)
    {
        return (await GetDailyRatesAsync(cancellationToken)).SingleOrDefault(x => x.Currency.Equals(currency, StringComparison.Ordinal));
    }
}
