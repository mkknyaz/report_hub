using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.GetCount;

public class GetInvoiceCountRequestValidator : AbstractValidator<GetInvoiceCountRequest>
{
    private readonly IValidator<InvoiceRevenueFilterDTO> _invoiceRevenueFilterDto;

    public GetInvoiceCountRequestValidator(IValidator<InvoiceRevenueFilterDTO> invoiceRevenueFilterDto)
    {
        _invoiceRevenueFilterDto = invoiceRevenueFilterDto;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x.InvoiceCountFilterDto)
            .SetValidator(_invoiceRevenueFilterDto);
    }
}
