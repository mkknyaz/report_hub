using AutoMapper;
using ErrorOr;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using MediatR;

namespace Exadel.ReportHub.Handlers.Invoice.GetRevenue;

public record GetInvoicesRevenueRequest(InvoiceRevenueFilterDTO InvoiceRevenueFilterDto) : IRequest<ErrorOr<TotalInvoicesRevenueDTO>>;

public class GetInvoicesRevenueHandler(
    IInvoiceRepository invoiceRepository,
    IClientRepository clientRepository,
    IMapper mapper) : IRequestHandler<GetInvoicesRevenueRequest, ErrorOr<TotalInvoicesRevenueDTO>>
{
    public async Task<ErrorOr<TotalInvoicesRevenueDTO>> Handle(GetInvoicesRevenueRequest request, CancellationToken cancellationToken)
    {
        var revenueResult = await invoiceRepository.GetTotalAmountByDateRangeAsync(
            request.InvoiceRevenueFilterDto.ClientId,
            request.InvoiceRevenueFilterDto.StartDate,
            request.InvoiceRevenueFilterDto.EndDate,
            cancellationToken);
        if (revenueResult is null)
        {
            var emptyRevenueResultDto = new TotalInvoicesRevenueDTO
            {
                CurrencyCode = await clientRepository.GetCurrencyAsync(request.InvoiceRevenueFilterDto.ClientId, cancellationToken)
            };

            return emptyRevenueResultDto;
        }

        var revenueResultDto = mapper.Map<TotalInvoicesRevenueDTO>(revenueResult);

        return revenueResultDto;
    }
}
