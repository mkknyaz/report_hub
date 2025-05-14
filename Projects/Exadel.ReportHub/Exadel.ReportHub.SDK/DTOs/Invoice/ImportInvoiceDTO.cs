namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class ImportInvoiceDTO : UpdateInvoiceDTO
{
    public Guid CustomerId { get; set; }

    public string InvoiceNumber { get; set; }

    public IList<Guid> ItemIds { get; set; }
}
