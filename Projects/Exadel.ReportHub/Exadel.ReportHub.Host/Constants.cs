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
            public const string ClientAdmin = "ClientAdmin";
            public const string SuperAdmin = "SuperAdmin";
            public const string AllUsers = "AllUsers";
        }
    }
}
