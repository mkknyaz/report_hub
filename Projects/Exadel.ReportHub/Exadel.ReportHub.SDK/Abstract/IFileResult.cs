namespace Exadel.ReportHub.SDK.Abstract;

public interface IFileResult
{
    public Stream Stream { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }
}
