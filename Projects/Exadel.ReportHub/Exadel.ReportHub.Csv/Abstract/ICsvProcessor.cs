using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Csv.Abstract;

public interface ICsvProcessor
{
    IList<CreateInvoiceDTO> ReadInvoices(Stream csvStream);
}
