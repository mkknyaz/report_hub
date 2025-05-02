using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.GetRevenue;

public record GetInvoicesRevenueRequest(InvoiceRevenueFilterDTO InvoiceRevenueFilterDto) : IRequest<ErrorOr<TotalInvoicesRevenueDTO>>;

public class GetInvoicesRevenueHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoicesRevenueRequest, ErrorOr<TotalInvoicesRevenueDTO>>
{
    public async Task<ErrorOr<TotalInvoicesRevenueDTO>> Handle(GetInvoicesRevenueRequest request, CancellationToken cancellationToken)
    {
        var (currencyCode, total) = await invoiceRepository.GetTotalAmountByDateRangeAsync(request.InvoiceRevenueFilterDto.ClientId,
            request.InvoiceRevenueFilterDto.StartDate, request.InvoiceRevenueFilterDto.EndDate, cancellationToken);

        return new TotalInvoicesRevenueDTO
        {
            TotalRevenue = total,
            CurrencyCode = currencyCode
        };
    }
}
