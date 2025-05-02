namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class InvoiceCountResultDTO
{
    public int Total { get; set; }

    public Dictionary<Guid, int> Customers { get; set; }
}
