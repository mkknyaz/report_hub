using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Get;

public record GetCustomersByClientIdRequest(Guid ClientId) : IRequest<ErrorOr<IList<CustomerDTO>>>;

public class GetCustomersByClientIdHandler(ICustomerRepository customerRepository, IMapper mapper) : IRequestHandler<GetCustomersByClientIdRequest, ErrorOr<IList<CustomerDTO>>>
{
    public async Task<ErrorOr<IList<CustomerDTO>>> Handle(GetCustomersByClientIdRequest request, CancellationToken cancellationToken)
    {
        var customers = await customerRepository.GetByClientIdAsync(request.ClientId, cancellationToken);

        var customersDto = mapper.Map<List<CustomerDTO>>(customers);
        return customersDto;
    }
}
