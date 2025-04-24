using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDTO>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IValidator<UpdateInvoiceDTO> _updateInvoiceValidator;

    public CreateInvoiceDtoValidator(IInvoiceRepository invoiceRepository, IClientRepository clientRepository, ICustomerRepository customerRepository,
        IItemRepository itemRepository, IValidator<UpdateInvoiceDTO> updateinvoiceValidator)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _customerRepository = customerRepository;
        _itemRepository = itemRepository;
        _updateInvoiceValidator = updateinvoiceValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x)
            .SetValidator(_updateInvoiceValidator);

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Invoice.ClientDoesntExistsErrorMessage);

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .MustAsync(_customerRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Invoice.CustomerDoesntExistsErrorMessage);

        RuleFor(x => x.InvoiceNumber)
            .NotEmpty()
            .MaximumLength(Constants.Validation.Invoice.InvoiceMaximumNumberLength)
            .Matches(@"^INV\d+$")
            .WithMessage(Constants.Validation.Invoice.InvoiceNumberErrorMessage)
            .MustAsync(InvoiceNumberMustNotExistAsync)
            .WithMessage(Constants.Validation.Invoice.InvoiceNumberExistsMessage);

        RuleFor(x => x.ItemIds)
            .NotEmpty()
            .Must(x => x.Count == x.Distinct().Count())
            .WithMessage(Constants.Validation.Invoice.ItemsDuplicateErrorMessage)
            .MustAsync(_itemRepository.AllExistAsync)
            .WithMessage(Constants.Validation.Invoice.ItemDoesNotExistsErrorMessage);
    }

    private async Task<bool> InvoiceNumberMustNotExistAsync(string invoiceNumber, CancellationToken cancellationToken)
    {
        var isExists = await _invoiceRepository.ExistsAsync(invoiceNumber, cancellationToken);
        return !isExists;
    }
}
