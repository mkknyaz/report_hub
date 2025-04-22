using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.GetById;

public record GetInvoiceByIdRequest(Guid Id) : IRequest<ErrorOr<InvoiceDTO>>;

public class GetInvoiceByIdHandler(IInvoiceRepository invoiceRepository, IMapper mapper) : IRequestHandler<GetInvoiceByIdRequest, ErrorOr<InvoiceDTO>>
{
    public async Task<ErrorOr<InvoiceDTO>> Handle(GetInvoiceByIdRequest request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice == null)
        {
            return Error.NotFound();
        }

        var invoiceDto = mapper.Map<InvoiceDTO>(invoice);
        return invoiceDto;
    }
}
