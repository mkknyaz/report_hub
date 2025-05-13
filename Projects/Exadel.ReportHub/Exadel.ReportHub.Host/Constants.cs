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

    public static class SwaggerSummary
    {
        public static class Common
        {
            public const string Status400Description = "Invalid data provided.";

            public const string Status401Description = "Authentication is required to access this endpoint.";

            public const string Status403Description = "User does not have permission to perform this action.";

            public const string Status500Description = "An unexpected error occurred.";
        }

        public static class AuditReport
        {
            public const string Status200RetrieveDescription = $"Audit report {Templates.Status200Retrieve}";

            public const string Status404Description = $"Audit report {Templates.Status404}";
        }

        public static class Client
        {
            public const string Status200RetrieveDescription = $"Client {Templates.Status200Retrieve}";

            public const string Status201ImportDescription = $"Clients {Templates.Status201Import}";
            public const string Status201CreateDescription = $"Client {Templates.Status201Create}";

            public const string Status204UpdateDescription = $"Client {Templates.Status204Update}";
            public const string Status204DeleteDescription = $"Client {Templates.Status204Delete}";

            public const string Status404Description = $"Client {Templates.Status404}";
        }

        public static class Country
        {
            public const string Status200RetrieveDescription = $"Country {Templates.Status200Retrieve}";
        }

        public static class Customer
        {
            public const string Status200RetrieveDescription = $"Customer {Templates.Status200Retrieve}";

            public const string Status201ImportDescription = $"Customers {Templates.Status201Import}";
            public const string Status201CreateDescription = $"Customer {Templates.Status201Create}";

            public const string Status204UpdateDescription = $"Customer {Templates.Status204Update}";
            public const string Status204DeleteDescription = $"Customer {Templates.Status204Delete}";

            public const string Status404Description = $"Customer {Templates.Status404}";
        }

        public static class Invoice
        {
            public const string Status200RetrieveDescription = $"Invoice {Templates.Status200Retrieve}";
            public const string Status200ExportDescription = $"Invoice {Templates.Status200Export}";

            public const string Status201ImportDescription = $"Invoices {Templates.Status201Import}";
            public const string Status201CreateDescription = $"Invoice {Templates.Status201Create}";

            public const string Status204UpdateDescription = $"Invoice {Templates.Status204Update}";
            public const string Status204DeleteDescription = $"Invoice {Templates.Status204Delete}";

            public const string Status404Description = $"Invoice {Templates.Status404}";
        }

        public static class Item
        {
            public const string Status200RetrieveDescription = $"Item {Templates.Status200Retrieve}";

            public const string Status201CreateDescription = $"Item {Templates.Status201Create}";

            public const string Status204UpdateDescription = $"Item {Templates.Status204Update}";
            public const string Status204DeleteDescription = $"Item {Templates.Status204Delete}";

            public const string Status404Description = $"Item {Templates.Status404}";
        }

        public static class Plan
        {
            public const string Status200RetrieveDescription = $"Plan {Templates.Status200Retrieve}";

            public const string Status201CreateDescription = $"Plan {Templates.Status201Create}";

            public const string Status204UpdateDescription = $"Plan {Templates.Status204Update}";
            public const string Status204DeleteDescription = $"Plan {Templates.Status204Delete}";

            public const string Status404Description = $"Plan {Templates.Status404}";
        }

        public static class Report
        {
            public const string Status200ExportDescription = $"Report {Templates.Status200Export}";
        }

        public static class UserAssignment
        {
            public const string Status204UpdateDescription = $"User assignment {Templates.Status204Update}";
        }

        public static class User
        {
            public const string Status200RetrieveDescription = $"User {Templates.Status200Retrieve}";

            public const string Status201CreateDescription = $"User {Templates.Status201Create}";

            public const string Status204UpdateDescription = $"User {Templates.Status204Update}";
            public const string Status204DeleteDescription = $"User {Templates.Status204Delete}";

            public const string Status404Description = $"User {Templates.Status404}";
        }

        private static class Templates
        {
            public const string Status200Retrieve = "data was retrieved successfully.";
            public const string Status200Export = "was exported successfully.";

            public const string Status201Create = "was created successfully.";
            public const string Status201Import = "were imported successfully.";

            public const string Status204Update = "was updated successfully.";
            public const string Status204Delete = "was deleted successfully.";

            public const string Status404 = "was not found.";
        }
    }
}
