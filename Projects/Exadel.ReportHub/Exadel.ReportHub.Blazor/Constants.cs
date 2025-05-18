namespace Exadel.ReportHub.Blazor;

public static class Constants
{
    public static class Authentication
    {
        public const string ClientId = "report_hub_resource_owner";
        public const string Scope = "report_hub_api";
    }

    public static class Storage
    {
        public const string SetItem = "sessionStorage.setItem";
        public const string GetItem = "sessionStorage.getItem";
        public const string RemoveItem = "sessionStorage.removeItem";
    }
}
