namespace Exadel.ReportHub.SDK.DTOs.Customer;

public class CustomerDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public Guid CountryId { get; set; }

    public string Country { get; set; }

    public Guid CurrencyId { get; set; }

    public string CurrencyCode { get; set; }
}
