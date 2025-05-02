using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.GetTotalRevenue;

public record GetInvoicesTotalRevenueRequest(InvoiceIssueDateFilterDTO InvoiceDateFilterDto) : IRequest<ErrorOr<TotalInvoicesRevenueDTO>>;

public class GetInvoicesTotalRevenueHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoicesTotalRevenueRequest, ErrorOr<TotalInvoicesRevenueDTO>>
{
    public async Task<ErrorOr<TotalInvoicesRevenueDTO>> Handle(GetInvoicesTotalRevenueRequest request, CancellationToken cancellationToken)
    {
        var sumOfInvoicesAmount = await invoiceRepository.SumOfClientAmountAsync(request.InvoiceDateFilterDto.ClientId,
            request.InvoiceDateFilterDto.StartDate, request.InvoiceDateFilterDto.EndDate, cancellationToken);

        TotalInvoicesRevenueDTO totalRevenueResult = new()
        {
            TotalRevenue = sumOfInvoicesAmount.Total,
            CurrencyCode = sumOfInvoicesAmount.CurrencyCode
        };
        return totalRevenueResult;
    }
}
