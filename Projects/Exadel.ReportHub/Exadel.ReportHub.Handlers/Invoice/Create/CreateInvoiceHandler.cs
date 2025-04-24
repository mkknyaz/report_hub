using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.Handlers.Managers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.Create;

public record CreateInvoiceRequest(CreateInvoiceDTO CreateInvoiceDto) : IRequest<ErrorOr<InvoiceDTO>>;

public class CreateInvoiceHandler(
    IInvoiceRepository invoiceRepository,
    IInvoiceManager invoiceManager,
    IMapper mapper) : IRequestHandler<CreateInvoiceRequest, ErrorOr<InvoiceDTO>>
{
    public async Task<ErrorOr<InvoiceDTO>> Handle(CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var invoice = await invoiceManager.GenerateInvoiceAsync(request.CreateInvoiceDto, cancellationToken);

        await invoiceRepository.AddAsync(invoice, cancellationToken);
        return mapper.Map<InvoiceDTO>(invoice);
    }
}
