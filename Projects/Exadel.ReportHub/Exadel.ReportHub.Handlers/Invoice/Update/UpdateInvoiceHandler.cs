using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.Update;

public record UpdateInvoiceRequest(Guid Id, Guid ClientId, UpdateInvoiceDTO UpdateInvoiceDto) : IRequest<ErrorOr<Updated>>;

public class UpdateInvoiceHandler(IInvoiceRepository invoiceRepository, IMapper mapper) : IRequestHandler<UpdateInvoiceRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateInvoiceRequest request, CancellationToken cancellationToken)
    {
        var isExists = await invoiceRepository.ExistsAsync(request.Id, request.ClientId, cancellationToken);
        if (!isExists)
        {
            return Error.NotFound();
        }

        var invoice = mapper.Map<Data.Models.Invoice>(request.UpdateInvoiceDto);
        invoice.Id = request.Id;
        await invoiceRepository.UpdateAsync(invoice, cancellationToken);
        return Result.Updated;
    }
}
