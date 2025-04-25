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
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .SetValidator(_updateInvoiceValidator);

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Client.DoesNotExist);

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .MustAsync(_customerRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Customer.DoesNotExist);

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

        RuleFor(x => x.ClientId)
            .MustAsync(async (dto, clientId, cancellationToken) => clientId == await _customerRepository.GetClientIdAsync(dto.CustomerId, cancellationToken))
            .WithMessage(Constants.Validation.Customer.WrongClient);
    }
}
