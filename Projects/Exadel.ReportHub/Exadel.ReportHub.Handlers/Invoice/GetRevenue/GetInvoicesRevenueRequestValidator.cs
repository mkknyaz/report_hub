using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.GetRevenue;

public class GetInvoicesRevenueRequestValidator : AbstractValidator<GetInvoicesRevenueRequest>
{
    private readonly IValidator<InvoiceRevenueFilterDTO> _invoiceRevenueFilterDto;

    public GetInvoicesRevenueRequestValidator(IValidator<InvoiceRevenueFilterDTO> invoiceRevenueFilterDto)
    {
        _invoiceRevenueFilterDto = invoiceRevenueFilterDto;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.InvoiceRevenueFilterDto)
            .SetValidator(_invoiceRevenueFilterDto);
    }
}
