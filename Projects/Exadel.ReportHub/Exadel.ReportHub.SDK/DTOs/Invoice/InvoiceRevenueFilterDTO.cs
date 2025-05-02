namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class InvoiceRevenueFilterDTO
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public Guid ClientId { get; set; }
}
