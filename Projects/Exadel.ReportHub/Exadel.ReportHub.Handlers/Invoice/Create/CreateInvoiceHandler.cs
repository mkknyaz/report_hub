using ErrorOr;
using Exadel.ReportHub.Handlers.Managers.Invoice;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.Create;

public record CreateInvoiceRequest(CreateInvoiceDTO CreateInvoiceDto) : IRequest<ErrorOr<InvoiceDTO>>;

public class CreateInvoiceHandler(IInvoiceManager invoiceManager) : IRequestHandler<CreateInvoiceRequest, ErrorOr<InvoiceDTO>>
{
    public async Task<ErrorOr<InvoiceDTO>> Handle(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        return await invoiceManager.CreateInvoiceAsync(request.CreateInvoiceDto, cancellationToken);
    }
}
