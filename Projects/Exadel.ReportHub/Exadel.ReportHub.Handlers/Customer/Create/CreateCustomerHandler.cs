using ErrorOr;
using Exadel.ReportHub.Handlers.Managers.Customer;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Create;

public record CreateCustomerRequest(CreateCustomerDTO CreateCustomerDTO) : IRequest<ErrorOr<CustomerDTO>>;

public class CreateCustomerHandler(ICustomerManager customerManager) : IRequestHandler<CreateCustomerRequest, ErrorOr<CustomerDTO>>
{
    public async Task<ErrorOr<CustomerDTO>> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        return await customerManager.CreateCustomerAsync(request.CreateCustomerDTO, cancellationToken);
    }
}
