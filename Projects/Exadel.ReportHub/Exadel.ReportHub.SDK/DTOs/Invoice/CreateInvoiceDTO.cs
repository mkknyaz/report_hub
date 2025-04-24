namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class CreateInvoiceDTO : UpdateInvoiceDTO
{
    public Guid ClientId { get; set; }

    public Guid CustomerId { get; set; }

    public string InvoiceNumber { get; set; }

    public IList<Guid> ItemIds { get; set; }
}
