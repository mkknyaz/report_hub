using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.Create;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceRequest>
{
    private readonly IValidator<CreateInvoiceDTO> _invoiceValidator;

    public CreateInvoiceValidator(IValidator<CreateInvoiceDTO> invoiceValidator)
    {
        _invoiceValidator = invoiceValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x.CreateInvoiceDto)
            .SetValidator(_invoiceValidator);
    }
}
