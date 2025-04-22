using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.GetByClientId;

public record GetInvoicesByClientIdRequest(Guid ClientId) : IRequest<ErrorOr<IList<InvoiceDTO>>>;

public class GetInvoicesByClientIdHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    : IRequestHandler<GetInvoicesByClientIdRequest, ErrorOr<IList<InvoiceDTO>>>
{
    public async Task<ErrorOr<IList<InvoiceDTO>>> Handle(GetInvoicesByClientIdRequest request, CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.GetByClientIdAsync(request.ClientId, cancellationToken);
        var invoicesDto = mapper.Map<List<InvoiceDTO>>(invoices);
        return invoicesDto;
    }
}
