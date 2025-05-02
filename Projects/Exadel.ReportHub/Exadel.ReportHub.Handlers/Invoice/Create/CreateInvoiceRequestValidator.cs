using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.Create;

public class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    private readonly IValidator<CreateInvoiceDTO> _invoiceValidator;

    public CreateInvoiceRequestValidator(IValidator<CreateInvoiceDTO> invoiceValidator)
    {
        _invoiceValidator = invoiceValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.CreateInvoiceDto)
            .SetValidator(_invoiceValidator);
    }
}
