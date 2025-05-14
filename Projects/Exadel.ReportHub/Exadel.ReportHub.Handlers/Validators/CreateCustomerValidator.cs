using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDTO>
{
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<ImportCustomerDTO> _importCustomerValidator;

    public CreateCustomerValidator(IClientRepository clientRepository, IValidator<ImportCustomerDTO> importCustomerValidator)
    {
        _clientRepository = clientRepository;
        _importCustomerValidator = importCustomerValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
           .SetValidator(_importCustomerValidator);

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Client.DoesNotExist);
    }
}
