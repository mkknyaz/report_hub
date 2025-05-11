namespace Exadel.ReportHub.Csv.Abstract;

public interface ICsvImporter
{
    IList<TResult> Read<TResult>(Stream csvStream);
}
