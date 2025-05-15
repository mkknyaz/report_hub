namespace Exadel.ReportHub.Export.Abstract;

public static class Constants
{
    public static class Format
    {
        public const string Date = "yyyy-MM-dd";
        public const string Decimal = "0.00";
    }

    public static class File
    {
        public static class Extension
        {
            public const string Pdf = ".pdf";
            public const string Csv = ".csv";
            public const string Excel = ".xlsx";
        }

        public static class ContentType
        {
            public const string Excel = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }

    public static class ChartInfo
    {
        public const string PlansChartTitle = "Revenue Trend ({0})";
        public const string PlansNamesTitle = "Item";
        public const string PlansValuesTitle = "Revenue";
    }
}
