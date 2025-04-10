using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.SDK.Enums;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class InvoiceValidator : AbstractValidator<CreateInvoiceDTO>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IClientRepository _clientRepository;

    public InvoiceValidator(ICustomerRepository customerRepository, IClientRepository clientRepository)
    {
        _customerRepository = customerRepository;
        _clientRepository = clientRepository;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

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
            .WithMessage(Constants.Validation.Invoice.InvoiceNumberErrorMessage);

        RuleFor(x => x.IssueDate)
            .NotEmpty()
            .LessThan(DateTime.UtcNow)
            .WithMessage(Constants.Validation.Invoice.IssueDateErrorMessage);

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .GreaterThan(x => x.IssueDate)
            .WithMessage(Constants.Validation.Invoice.DueDateErrorMessage);

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(Constants.Validation.Invoice.CurrencyCodeLength)
            .WithMessage($"Currency must be exactly {Constants.Validation.Invoice.CurrencyCodeLength} characters long.")
            .Matches(@"^[A-Z]+$")
            .WithMessage($"Currency code must be exactly {Constants.Validation.Invoice.CurrencyCodeLength} uppercase letters.");

        RuleFor(x => x.PaymentStatus)
            .IsInEnum();

        RuleFor(x => x.BankAccountNumber)
            .NotEmpty()
            .Length(Constants.Validation.Invoice.BankAccountNumberMinLength, Constants.Validation.Invoice.BankAccountNumberMaxLength)
            .Matches(@"^[0-9\-]+$")
            .WithMessage(Constants.Validation.Invoice.BankAccountNumberErrorMessage);

        RuleFor(x => x.ItemIds)
            .NotEmpty();
    }
}
