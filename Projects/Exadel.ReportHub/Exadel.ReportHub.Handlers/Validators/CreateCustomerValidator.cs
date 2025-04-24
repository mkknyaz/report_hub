using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerDTO>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<UpdateCustomerDTO> _updateCustomerValidator;

    public CreateCustomerValidator(ICustomerRepository customerRepository, IValidator<UpdateCustomerDTO> updateCustomerValidator)
    {
        _customerRepository = customerRepository;
        _updateCustomerValidator = updateCustomerValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x)
           .SetValidator(_updateCustomerValidator);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(Constants.Validation.Customer.EmailInvalidMessage)
            .MustAsync(EmailMustNotExistsAsync)
            .WithMessage(Constants.Validation.Customer.EmailTakenMessage);
    }

    private async Task<bool> EmailMustNotExistsAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await _customerRepository.EmailExistsAsync(email, cancellationToken);
        return !emailExists;
    }
}
