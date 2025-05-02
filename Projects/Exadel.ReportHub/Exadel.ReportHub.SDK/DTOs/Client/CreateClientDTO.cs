namespace Exadel.ReportHub.SDK.DTOs.Client;

public class CreateClientDTO
{
    public string Name { get; set; }

    public string BankAccountNumber { get; set; }

    public Guid CountryId { get; set; }
}
