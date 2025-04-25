namespace Exadel.ReportHub.SDK.DTOs.Customer;

public class CreateCustomerDTO : UpdateCustomerDTO
{
    public string Email { get; set; }

    public Guid ClientId { get; set; }
}
