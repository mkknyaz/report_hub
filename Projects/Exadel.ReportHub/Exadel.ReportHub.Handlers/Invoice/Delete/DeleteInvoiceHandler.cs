using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.Delete;

public record DeleteInvoiceRequest(Guid Id, Guid ClientId) : IRequest<ErrorOr<Deleted>>;

public class DeleteInvoiceHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<DeleteInvoiceRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteInvoiceRequest request, CancellationToken cancellationToken)
    {
        var isExists = await invoiceRepository.ExistsAsync(request.Id, request.ClientId, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await invoiceRepository.SoftDeleteAsync(request.Id, cancellationToken);
        return Result.Deleted;
    }
}
