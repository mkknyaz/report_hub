using Exadel.ReportHub.SDK.Abstract;

namespace Exadel.ReportHub.Export.Abstract;

public class ExportResult : IFileResult
{
    public Stream Stream { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }
}
