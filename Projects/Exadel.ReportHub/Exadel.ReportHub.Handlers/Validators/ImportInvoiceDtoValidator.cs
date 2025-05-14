using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class ImportInvoiceDtoValidator : AbstractValidator<ImportInvoiceDTO>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IValidator<UpdateInvoiceDTO> _updateInvoiceValidator;

    public ImportInvoiceDtoValidator(IInvoiceRepository invoiceRepository,
        IItemRepository itemRepository, IValidator<UpdateInvoiceDTO> updateInvoiceValidator)
    {
        _invoiceRepository = invoiceRepository;
        _itemRepository = itemRepository;
        _updateInvoiceValidator = updateInvoiceValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .SetValidator(_updateInvoiceValidator);

        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.InvoiceNumber)
            .NotEmpty()
            .MaximumLength(Constants.Validation.Invoice.InvoiceNumberMaxLength)
            .Matches(@"^INV\d+$")
            .WithMessage(Constants.Validation.Invoice.InvalidInvoiceNumberFormat)
            .MustAsync(async (number, cancellationToken) => !await _invoiceRepository.ExistsAsync(number, cancellationToken))
            .WithMessage(Constants.Validation.Invoice.DuplicateInvoice);

        RuleFor(x => x.ItemIds)
            .NotEmpty()
            .Must(x => x.Count == x.Distinct().Count())
            .WithMessage(Constants.Validation.Invoice.DuplicateItem)
            .MustAsync(_itemRepository.AllExistAsync)
            .WithMessage(Constants.Validation.Item.DoesNotExist);
    }
}
