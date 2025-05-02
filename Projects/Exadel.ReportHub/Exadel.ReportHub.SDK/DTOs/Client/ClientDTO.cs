namespace Exadel.ReportHub.SDK.DTOs.Client;

public class ClientDTO
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string BankAccountNumber { get; set; }

    public Guid CountryId { get; set; }

    public string Country { get; set; }

    public Guid CurrencyId { get; set; }

    public string CurrencyCode { get; set; }
}
