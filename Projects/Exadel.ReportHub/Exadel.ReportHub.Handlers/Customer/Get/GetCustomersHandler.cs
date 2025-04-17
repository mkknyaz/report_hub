using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Get;

public record GetCustomersRequest : IRequest<ErrorOr<IEnumerable<CustomerDTO>>>;

public class GetCustomersHandler(ICustomerRepository customerRepository, IMapper mapper) : IRequestHandler<GetCustomersRequest, ErrorOr<IEnumerable<CustomerDTO>>>
{
    public async Task<ErrorOr<IEnumerable<CustomerDTO>>> Handle(GetCustomersRequest request, CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetAsync(cancellationToken);

        var customersDto = mapper.Map<List<CustomerDTO>>(customers);
        return customersDto;
    }
}
