using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.Create;

public record CreateInvoiceRequest(CreateInvoiceDTO CreateInvoiceDto) : IRequest<ErrorOr<InvoiceDTO>>;

public class CreateInvoiceHandler(IInvoiceRepository invoiceRepository, IMapper mapper) : IRequestHandler<CreateInvoiceRequest, ErrorOr<InvoiceDTO>>
{
    public async Task<ErrorOr<InvoiceDTO>> Handle(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = mapper.Map<Data.Models.Invoice>(request.CreateInvoiceDto);
        invoice.Id = Guid.NewGuid();
        await invoiceRepository.AddAsync(invoice, cancellationToken);
        return mapper.Map<InvoiceDTO>(invoice);
    }
}
