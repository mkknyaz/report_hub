using ErrorOr;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Delete;

public record DeleteCustomerRequest(Guid CustomerId, Guid ClientId) : IRequest<ErrorOr<Deleted>>;

public class DeleteCustomerHandler(ICustomerRepository customerRepository) : IRequestHandler<DeleteCustomerRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var isExists = await customerRepository.ExistsAsync(request.CustomerId, request.ClientId, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await customerRepository.SoftDeleteAsync(request.CustomerId, cancellationToken);
        return Result.Deleted;
    }
}
