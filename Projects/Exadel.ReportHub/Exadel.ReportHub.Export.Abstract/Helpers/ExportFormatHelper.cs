using System.Net.Mime;
using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Export.Abstract.Helpers;

public static class ExportFormatHelper
{
    public static string GetFileExtension(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.CSV => Constants.File.Extension.Csv,
            ExportFormat.Excel => Constants.File.Extension.Excel,
            _ => throw new ArgumentException($"Unsupported export format: {format}")
        };
    }

    public static string GetContentType(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.CSV => MediaTypeNames.Text.Csv,
            ExportFormat.Excel => Constants.File.ContentType.Excel,
            _ => throw new ArgumentException($"Unsupported export format: {format}")
        };
    }
}
