using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.GetById;

public record GetCustomerByIdRequest(Guid Id) : IRequest<ErrorOr<CustomerDTO>>;

public class GetCustomerByIdHandler(ICustomerRepository customerRepository, IMapper mapper) : IRequestHandler<GetCustomerByIdRequest, ErrorOr<CustomerDTO>>
{
    public async Task<ErrorOr<CustomerDTO>> Handle(GetCustomerByIdRequest request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            return Error.NotFound();
        }

        var customerDto = mapper.Map<CustomerDTO>(customer);
        return customerDto;
    }
}
