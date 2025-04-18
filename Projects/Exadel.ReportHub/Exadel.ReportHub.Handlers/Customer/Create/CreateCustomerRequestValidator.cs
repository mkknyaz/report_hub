using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Customer.Create;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<string> _stringValidator;

    public CreateCustomerRequestValidator(ICustomerRepository customerRepository, IValidator<string> stringValidator)
    {
        _customerRepository = customerRepository;
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CreateCustomerDTO)
            .ChildRules(child =>
            {
                child.RuleLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.Name)
                    .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names);

                child.RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .WithMessage(Constants.Validation.Customer.EmailInvalidMessage)
                    .MustAsync(EmailMustNotExistsAsync)
                    .WithMessage(Constants.Validation.Customer.EmailTakenMessage);

                child.RuleFor(x => x.Country)
                    .SetValidator(_stringValidator, Constants.Validation.RuleSet.Countries);
            });
    }

    private async Task<bool> EmailMustNotExistsAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await _customerRepository.EmailExistsAsync(email, cancellationToken);
        return !emailExists;
    }
}
