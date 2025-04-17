using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Customer.Delete;

public record DeleteCustomerRequest(Guid Id) : IRequest<ErrorOr<Deleted>>;

public class DeleteCustomerHandler(ICustomerRepository customerRepository) : IRequestHandler<DeleteCustomerRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var isExists = await customerRepository.ExistsAsync(request.Id, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await customerRepository.SoftDeleteAsync(request.Id, cancellationToken);
        return Result.Deleted;
    }
}
