namespace Exadel.ReportHub.Data.Models;

public class OverdueCount
{
    public int Count { get; set; }

    public decimal TotalAmount { get; set; }

    public string ClientCurrencyCode { get; set; }
}
