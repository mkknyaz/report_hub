using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Create;

public record CreateCustomerRequest(CreateCustomerDTO CreateCustomerDTO) : IRequest<ErrorOr<CustomerDTO>>;

public class CreateCustomerHandler(ICustomerRepository customerRepository, ICountryRepository countryRepository, IMapper mapper)
    : IRequestHandler<CreateCustomerRequest, ErrorOr<CustomerDTO>>
{
    public async Task<ErrorOr<CustomerDTO>> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = mapper.Map<Data.Models.Customer>(request.CreateCustomerDTO);
        var country = await countryRepository.GetByIdAsync(request.CreateCustomerDTO.CountryId, cancellationToken);

        customer.Id = Guid.NewGuid();
        customer.Country = country.Name;
        customer.CurrencyId = country.CurrencyId;
        customer.CurrencyCode = country.CurrencyCode;

        await customerRepository.AddAsync(customer, cancellationToken);
        return mapper.Map<CustomerDTO>(customer);
    }
}
