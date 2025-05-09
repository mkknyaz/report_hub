namespace Exadel.ReportHub.Csv.Abstract;

public interface ICsvImporter
{
    IList<TResult> ReadInvoices<TResult>(Stream csvStream);
}
