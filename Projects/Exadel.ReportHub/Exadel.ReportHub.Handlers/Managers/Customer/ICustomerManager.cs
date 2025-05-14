using Exadel.ReportHub.SDK.DTOs.Customer;

namespace Exadel.ReportHub.Handlers.Managers.Customer;

public interface ICustomerManager
{
    Task<CustomerDTO> CreateCustomerAsync(CreateCustomerDTO createCustomerDto, CancellationToken cancellationToken);

    Task<IList<CustomerDTO>> CreateCustomersAsync(IEnumerable<CreateCustomerDTO> createCustomerDtos, CancellationToken cancellationToken);
}
