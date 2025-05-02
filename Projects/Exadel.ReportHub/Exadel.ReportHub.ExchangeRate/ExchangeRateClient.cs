using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Ecb.Abstract;
using Microsoft.Extensions.Logging;

namespace Exadel.ReportHub.Ecb;

public class ExchangeRateClient(IHttpClientFactory factory, ILogger<ExchangeRateClient> logger) : IExchangeRateClient
{
    public async Task<IList<ExchangeRate>> GetByCurrencyInPeriodAsync(string currency, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var client = factory.CreateClient(Constants.ClientName);

        var response = await client.GetAsync(new Uri(string.Format(Constants.Path.ExchangeRatePathTemplate, currency,
            FormatDate(startDate), FormatDate(endDate)), UriKind.Relative), cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync(cancellationToken);

        XDocument document;
        try
        {
            document = XDocument.Parse(result);
        }
        catch (XmlException ex)
        {
            logger.LogError(ex, Constants.Error.EmptyXml);
            return new List<ExchangeRate>();
        }

        var generic = document.Root.GetNamespaceOfPrefix("generic");
        var series = document.Descendants(generic + "Series").SingleOrDefault();

        var rates = series.Elements(generic + "Obs")
            .Select(x => new ExchangeRate
            {
                Id = Guid.NewGuid(),
                Currency = currency,
                Rate = decimal.Parse(x.Element(generic + "ObsValue").Attribute("value").Value, CultureInfo.InvariantCulture),
                RateDate = DateTime.Parse(x.Element(generic + "ObsDimension").Attribute("value").Value,
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal)
            })
            .ToList();

        return rates;
    }

    private string FormatDate(DateTime date)
    {
        return date.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
