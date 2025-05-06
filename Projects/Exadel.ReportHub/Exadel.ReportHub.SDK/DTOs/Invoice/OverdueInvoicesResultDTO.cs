namespace Exadel.ReportHub.SDK.DTOs.Invoice;

public class OverdueInvoicesResultDTO
{
    public int Count { get; set; }

    public decimal TotalAmount { get; set; }

    public string ClientCurrencyCode { get; set; }
}
