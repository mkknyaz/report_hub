using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.Enums;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.UpdatePaidStatus;

public record UpdateInvoicePaidStatusRequest(Guid Id, Guid ClientId) : IRequest<ErrorOr<Updated>>;

public class UpdateInvoicePaidStatusHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<UpdateInvoicePaidStatusRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateInvoicePaidStatusRequest request, CancellationToken cancellationToken)
    {
        var isExists = await invoiceRepository.ExistsAsync(request.Id, request.ClientId, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        await invoiceRepository.UpdatePaidStatusAsync(request.Id, request.ClientId, cancellationToken);

        return Result.Updated;
    }
}
