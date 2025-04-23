using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDTO>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IValidator<UpdateInvoiceDTO> _updateInvoiceValidator;

    public CreateInvoiceDtoValidator(ICustomerRepository customerRepository, IClientRepository clientRepository,
        IInvoiceRepository invoiceRepository, IValidator<UpdateInvoiceDTO> updateinvoiceValidator)
    {
        _updateInvoiceValidator = updateinvoiceValidator;
        _customerRepository = customerRepository;
        _clientRepository = clientRepository;
        _invoiceRepository = invoiceRepository;
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
            .NotEmpty();
    }

    private async Task<bool> InvoiceNumberMustNotExistAsync(string invoiceNumber, CancellationToken cancellationToken)
    {
        var isExists = await _invoiceRepository.ExistsAsync(invoiceNumber, cancellationToken);
        return !isExists;
    }
}
