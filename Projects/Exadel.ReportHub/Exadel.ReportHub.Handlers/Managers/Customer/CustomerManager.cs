using AutoMapper;
using Exadel.ReportHub.Handlers.Managers.Helpers;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.Customer;

namespace Exadel.ReportHub.Handlers.Managers.Customer;

public class CustomerManager(ICustomerRepository customerRepository, ICountryDataFiller countryBasedHelper, IMapper mapper) : ICustomerManager
{
    public async Task<CustomerDTO> CreateCustomerAsync(CreateCustomerDTO createCustomerDto, CancellationToken cancellationToken)
    {
        return (await CreateCustomersAsync([createCustomerDto], cancellationToken)).Single();
    }

    public async Task<IList<CustomerDTO>> CreateCustomersAsync(IEnumerable<CreateCustomerDTO> createCustomerDtos, CancellationToken cancellationToken)
    {
        var customers = mapper.Map<IList<Data.Models.Customer>>(createCustomerDtos);

        await countryBasedHelper.FillCountryDataAsync(customers, cancellationToken);

        foreach (var customer in customers)
        {
            customer.Id = Guid.NewGuid();
        }

        await customerRepository.AddManyAsync(customers, cancellationToken);

        return mapper.Map<IList<CustomerDTO>>(customers);
    }
}
