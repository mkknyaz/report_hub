using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class ImportCustomerValidator : AbstractValidator<ImportCustomerDTO>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<UpdateCustomerDTO> _updateCustomerValidator;

    public ImportCustomerValidator(ICustomerRepository customerRepository, IValidator<UpdateCustomerDTO> updateCustomerValidator)
    {
        _customerRepository = customerRepository;
        _updateCustomerValidator = updateCustomerValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .SetValidator(_updateCustomerValidator);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(Constants.Validation.Email.IsInvalid)
            .MustAsync(async (email, cancellationToken) => !await _customerRepository.EmailExistsAsync(email, cancellationToken))
            .WithMessage(Constants.Validation.Email.IsTaken);
    }
}
