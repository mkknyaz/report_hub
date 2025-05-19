using Exadel.ReportHub.SDK.DTOs.Customer;

namespace Exadel.ReportHub.Blazor.UI.Services.Abstractions;

public interface ICustomersService
{
    Task<IList<CustomerDTO>> GetCustomersByClientIdAsync(Guid clientId);

    Task<CustomerDTO> GetCustomerByIdAsync(Guid customerId, Guid clientId);

    Task<CustomerDTO> CreateCustomerAsync(CreateCustomerDTO dto);

    Task UpdateCustomerAsync(Guid customerId, UpdateCustomerDTO dto, Guid clientId);

    Task DeleteCustomerAsync(Guid customerId, Guid clientId);
}
