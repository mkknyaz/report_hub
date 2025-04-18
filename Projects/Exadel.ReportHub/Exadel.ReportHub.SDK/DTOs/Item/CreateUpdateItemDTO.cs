namespace Exadel.ReportHub.SDK.DTOs.Item;

public class CreateUpdateItemDTO
{
    public Guid ClientId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public Guid CurrencyId { get; set; }
}
