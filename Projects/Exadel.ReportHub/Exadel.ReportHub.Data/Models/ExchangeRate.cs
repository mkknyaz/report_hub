namespace Exadel.ReportHub.Data.Models;

public class ExchangeRate
{
    public Guid Id { get; set; }

    public string Currency { get; set; }

    public decimal Rate { get; set; }

    public DateTime RateDate { get; set; }
}
