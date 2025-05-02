namespace Exadel.ReportHub.Ecb;

public static class Constants
{
    public const string ClientName = "ExchangeRateClient";
    public const int WeeksLimit = 4;

    public static class Currency
    {
        public const string DefaultCurrencyCode = "EUR";
    }

    public static class Error
    {
        public const string EmptyXml = "Xml is empty";
    }

    public static class Path
    {
        public const string ExchangeRatePathTemplate = "/service/data/EXR/D.{0}.EUR.SP00.A?startPeriod={1}&endPeriod={2}";
    }
}
