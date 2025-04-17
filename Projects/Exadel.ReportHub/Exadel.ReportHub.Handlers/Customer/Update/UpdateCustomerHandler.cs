using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Update;

public record UpdateCustomerRequest(Guid Id, UpdateCustomerDTO UpdateCustomerDto) : IRequest<ErrorOr<Updated>>;

public class UpdateCustomerHandler(ICustomerRepository customerRepository, IMapper mapper) : IRequestHandler<UpdateCustomerRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var isExists = await customerRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        var customer = mapper.Map<Data.Models.Customer>(request.UpdateCustomerDto);
        customer.Id = request.Id;
        await customerRepository.UpdateAsync(customer, cancellationToken);
        return Result.Updated;
    }
}
