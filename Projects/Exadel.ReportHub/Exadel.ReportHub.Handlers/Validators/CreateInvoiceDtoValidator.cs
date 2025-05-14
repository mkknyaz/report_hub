using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CreateInvoiceDtoValidator : AbstractValidator<CreateInvoiceDTO>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<ImportInvoiceDTO> _importInvoiceValidator;

    public CreateInvoiceDtoValidator(IClientRepository clientRepository, ICustomerRepository customerRepository,
        IValidator<ImportInvoiceDTO> importInvoiceValidator)
    {
        _clientRepository = clientRepository;
        _customerRepository = customerRepository;
        _importInvoiceValidator = importInvoiceValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .SetValidator(_importInvoiceValidator);

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Client.DoesNotExist);

        RuleFor(x => x.CustomerId)
            .MustAsync(async (dto, customerId, cancellationToken) => await _customerRepository.ExistsOnClientAsync(customerId, dto.ClientId, cancellationToken))
            .WithMessage(Constants.Validation.Customer.DoesNotExistOnClient);
    }
}
