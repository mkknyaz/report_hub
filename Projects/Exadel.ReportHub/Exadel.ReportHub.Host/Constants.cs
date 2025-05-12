namespace Exadel.ReportHub.Host;

public static class Constants
{
    public static class Authorization
    {
        public const string ScopeName = "report_hub_api";
        public const string ScopeDescription = "Full access to Report Hub API";
        public const string ResourceName = "report_hub_api";

        public static class Policy
        {
            public const string Create = nameof(Create);
            public const string Read = nameof(Read);
            public const string Update = nameof(Update);
            public const string Delete = nameof(Delete);
            public const string Export = nameof(Export);
        }
    }

    public static class HttpClient
    {
        public const string PingClient = "PingClient";
        public const string ExchangeRateClient = "ExchangeRateClient";

        public static class Path
        {
            public static readonly Uri Ping = new Uri("/api/ping", UriKind.Relative);
        }
    }

    public static class Job
    {
        public static class Id
        {
            public const string SendUserReports = nameof(SendUserReports);
            public const string OverduePaymentStatusUpdater = nameof(OverduePaymentStatusUpdater);
            public const string Ping = nameof(Ping);
        }
    }
}
