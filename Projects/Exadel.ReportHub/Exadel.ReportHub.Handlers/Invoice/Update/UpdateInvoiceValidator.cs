using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.Update;

public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceRequest>
{
    private readonly IValidator<UpdateInvoiceDTO> _invoiceValidator;

    public UpdateInvoiceValidator(IValidator<UpdateInvoiceDTO> invoiceValidator)
    {
        _invoiceValidator = invoiceValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x.UpdateInvoiceDto)
            .SetValidator(_invoiceValidator);
    }
}
